```

BenchmarkDotNet v0.15.8, Linux Arch Linux
12th Gen Intel Core i5-1235U 0.40GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 10.0.111
  [Host]     : .NET 10.0.11 (10.0.11, 42.42.42.42424), X64 RyuJIT x86-64-v3
  Job-CNUJVU : .NET 10.0.11 (10.0.11, 42.42.42.42424), X64 RyuJIT x86-64-v3

InvocationCount=1  UnrollFactor=1  

```
| Method               | ProducerCount | ItemsPerProducer | Mean        | Error     | StdDev   | Allocated |
|--------------------- |-------------- |----------------- |------------:|----------:|---------:|----------:|
| **UnsafeLockQueue_MPSC** | **1**             | **1000000**          |    **65.33 ms** |  **3.518 ms** | **10.32 ms** |     **512 B** |
| **UnsafeLockQueue_MPSC** | **4**             | **1000000**          |   **683.49 ms** | **15.887 ms** | **46.09 ms** |    **1744 B** |
| **UnsafeLockQueue_MPSC** | **8**             | **1000000**          | **1,415.75 ms** | **29.682 ms** | **87.52 ms** |    **3184 B** |
