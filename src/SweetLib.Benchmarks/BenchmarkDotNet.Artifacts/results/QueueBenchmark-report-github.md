```

BenchmarkDotNet v0.15.8, Linux Arch Linux
12th Gen Intel Core i5-1235U 0.40GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 10.0.111
  [Host]     : .NET 10.0.11 (10.0.11, 42.42.42.42424), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.11 (10.0.11, 42.42.42.42424), X64 RyuJIT x86-64-v3


```
| Method                     | Count    | Mean      | Error    | StdDev   | Allocated |
|--------------------------- |--------- |----------:|---------:|---------:|----------:|
| UnsafeQueue_EnqueueDequeue | 10000000 | 318.99 ms | 0.856 ms | 0.801 ms |         - |
| Queue_EnqueueDequeue       | 10000000 |  23.29 ms | 0.112 ms | 0.105 ms |         - |
