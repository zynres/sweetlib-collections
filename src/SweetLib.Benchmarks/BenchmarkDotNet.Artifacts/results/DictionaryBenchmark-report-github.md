```

BenchmarkDotNet v0.15.8, Linux Arch Linux
12th Gen Intel Core i5-1235U 0.40GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 10.0.111
  [Host]     : .NET 10.0.11 (10.0.11, 42.42.42.42424), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.11 (10.0.11, 42.42.42.42424), X64 RyuJIT x86-64-v3


```
| Method               | Count  | Mean     | Error   | StdDev   | Median   | Gen0     | Allocated |
|--------------------- |------- |---------:|--------:|---------:|---------:|---------:|----------:|
| Dictionary_Get       | 100000 | 157.9 μs | 3.15 μs |  7.73 μs | 154.6 μs |        - |         - |
| UnsafeDictionary_Get | 100000 | 322.2 μs | 6.41 μs | 12.51 μs | 315.7 μs | 382.3242 | 2400000 B |
