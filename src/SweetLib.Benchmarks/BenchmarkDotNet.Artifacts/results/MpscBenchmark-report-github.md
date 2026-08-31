```

BenchmarkDotNet v0.15.8, Linux Arch Linux
12th Gen Intel Core i5-1235U 0.40GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 10.0.111
  [Host]     : .NET 10.0.11 (10.0.11, 42.42.42.42424), X64 RyuJIT x86-64-v3
  Job-CNUJVU : .NET 10.0.11 (10.0.11, 42.42.42.42424), X64 RyuJIT x86-64-v3

InvocationCount=1  UnrollFactor=1  

```
| Method                      | ProducerCount | ItemsPerProducer | Mean        | Error     | StdDev     | Median      | Gen0      | Gen1      | Gen2      | Allocated  |
|---------------------------- |-------------- |----------------- |------------:|----------:|-----------:|------------:|----------:|----------:|----------:|-----------:|
| **UnsafeConcurrentQueue**       | **1**             | **1000000**          |    **67.85 ms** |  **3.484 ms** |  **10.162 ms** |    **65.13 ms** |         **-** |         **-** |         **-** |      **664 B** |
| UnsafeRingBuffer_MPSC       | 1             | 1000000          |    90.50 ms |  1.792 ms |   4.118 ms |    91.11 ms |         - |         - |         - |      232 B |
| SystemThreadingChannel_MPSC | 1             | 1000000          |    62.14 ms |  1.690 ms |   4.957 ms |    61.53 ms |         - |         - |         - |   262736 B |
| **UnsafeConcurrentQueue**       | **4**             | **1000000**          |   **696.92 ms** | **15.321 ms** |  **44.692 ms** |   **699.10 ms** |         **-** |         **-** |         **-** |     **1744 B** |
| UnsafeRingBuffer_MPSC       | 4             | 1000000          |   506.45 ms | 11.979 ms |  35.321 ms |   506.93 ms |         - |         - |         - |      856 B |
| SystemThreadingChannel_MPSC | 4             | 1000000          |   532.67 ms | 22.905 ms |  67.537 ms |   522.84 ms | 2000.0000 | 2000.0000 | 2000.0000 | 33556272 B |
| **UnsafeConcurrentQueue**       | **8**             | **1000000**          | **1,463.73 ms** | **29.112 ms** |  **68.620 ms** | **1,475.93 ms** |         **-** |         **-** |         **-** |     **3184 B** |
| UnsafeRingBuffer_MPSC       | 8             | 1000000          | 1,164.47 ms | 55.313 ms | 163.092 ms | 1,143.57 ms |         - |         - |         - |     1688 B |
| SystemThreadingChannel_MPSC | 8             | 1000000          | 1,128.99 ms | 40.584 ms | 119.662 ms | 1,113.66 ms | 2000.0000 | 2000.0000 | 2000.0000 | 67111304 B |
