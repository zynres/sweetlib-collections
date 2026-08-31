using SweetLib.Collections.Unsafe.Concurrent.Queue;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using System.Threading.Channels;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class MpscBenchmark
{
    private const int Capacity = 16_777_216; //1_048_576;

    [Params(1, 4, 8)]
    public int ProducerCount { get; set; }

    [Params(1_000_000)]
    public int ItemsPerProducer { get; set; }
    
    private UnsafeConcurrentQueue<int> _lockQueue;
    private MpscUnsafeRingBuffer<int> _unsafeBuffer = null!;
    private Channel<int> _channel = null!;

    [IterationSetup]
    public void Setup()
    {
        _lockQueue = new UnsafeConcurrentQueue<int>(Capacity);
        _unsafeBuffer = new MpscUnsafeRingBuffer<int>(Capacity);

        var options = new BoundedChannelOptions(Capacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<int>(options);
    }

    [IterationCleanup]
    public void Cleanup()
    {
        _unsafeBuffer.Dispose();
        _lockQueue.Dispose();
    }

    [Benchmark]
    public void UnsafeConcurrentQueue()
    {
        int totalItems = ProducerCount * ItemsPerProducer;
        var writerThreads = new Thread[ProducerCount];

        for (int i = 0; i < ProducerCount; i++)
        {
            writerThreads[i] = new Thread(() =>
            {
                int count = ItemsPerProducer;
                for (int j = 0; j < count; j++)
                {
                    _lockQueue.Enqueue(j);
                }
            });
            writerThreads[i].Start();
        }

        int readCount = 0;
        while (readCount < totalItems)
        {
            if (_lockQueue.TryDequeue(out _))
            {
                readCount++;
            }
        }

        for (int i = 0; i < ProducerCount; i++)
        {
            writerThreads[i].Join();
        }
    }


    [Benchmark]
    public void UnsafeRingBuffer_MPSC()
    {
        int totalItems = ProducerCount * ItemsPerProducer;
        var writerThreads = new Thread[ProducerCount];

        for (int i = 0; i < ProducerCount; i++)
        {
            writerThreads[i] = new Thread(() =>
            {
                int count = ItemsPerProducer;
                for (int j = 0; j < count; j++)
                {
                    while (!_unsafeBuffer.TryEnqueue(j))
                    {
                        Thread.SpinWait(1);
                    }
                }
            });
            writerThreads[i].Start();
        }

        int readCount = 0;
        while (readCount < totalItems)
        {
            if (_unsafeBuffer.TryDequeue(out _))
            {
                readCount++;
            }
        }

        for (int i = 0; i < ProducerCount; i++)
        {
            writerThreads[i].Join();
        }
    }

    [Benchmark]
    public void SystemThreadingChannel_MPSC()
    {
        int totalItems = ProducerCount * ItemsPerProducer;
        var writerThreads = new Thread[ProducerCount];

        var writer = _channel.Writer;
        var reader = _channel.Reader;

        for (int i = 0; i < ProducerCount; i++)
        {
            writerThreads[i] = new Thread(() =>
            {
                int count = ItemsPerProducer;
                for (int j = 0; j < count; j++)
                {
                    while (!writer.TryWrite(j))
                    {
                        Thread.SpinWait(1);
                    }
                }
            });
            writerThreads[i].Start();
        }

        int readCount = 0;
        while (readCount < totalItems)
        {
            if (reader.TryRead(out _))
            {
                readCount++;
            }
        }

        for (int i = 0; i < ProducerCount; i++)
        {
            writerThreads[i].Join();
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<MpscBenchmark>();
    }
}

public unsafe class MpscUnsafeRingBuffer<T> : IDisposable where T : unmanaged
{
    private readonly T* _data;
    private readonly uint _capacity;
    private readonly uint _mask;

    private VolatileNode* _nodes;

    private long _writeHead = 0;
    private long _readHead = 0;

    [StructLayout(LayoutKind.Sequential)]
    private struct VolatileNode
    {
        public T Item;
        public volatile int IsWritten;
    }

    public MpscUnsafeRingBuffer(uint powerOfTwoCapacity)
    {
        if ((powerOfTwoCapacity & (powerOfTwoCapacity - 1)) != 0)
            throw new ArgumentException("Capacity must be a power of 2");

        _capacity = powerOfTwoCapacity;
        _mask = _capacity - 1;

        _nodes = (VolatileNode*)NativeMemory.AllocZeroed(_capacity, (nuint)sizeof(VolatileNode));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryEnqueue(in T item)
    {
        while (true)
        {
            long currentRead = Volatile.Read(ref _readHead);
            long currentWrite = Volatile.Read(ref _writeHead);

            if (currentWrite - currentRead >= _capacity)
                return false;

            if (Interlocked.CompareExchange(ref _writeHead, currentWrite + 1, currentWrite) == currentWrite)
            {
                uint index = (uint)(currentWrite & _mask);
                VolatileNode* node = &_nodes[index];

                node->Item = item;
                node->IsWritten = 1; 
                return true;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDequeue(out T item)
    {
        long currentRead = _readHead;
        uint index = (uint)(currentRead & _mask);
        VolatileNode* node = &_nodes[index];

        if (node->IsWritten == 1)
        {
            item = node->Item;
            node->IsWritten = 0;
            _readHead++;
            return true;
        }

        item = default;
        return false;
    }

    public void Dispose()
    {
        if (_nodes != null)
        {
            NativeMemory.Free(_nodes);
            _nodes = null;
        }
    }
}
