```

BenchmarkDotNet v0.15.8, Linux Arch Linux
12th Gen Intel Core i5-1235U 0.40GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 10.0.111
  [Host]     : .NET 10.0.11 (10.0.11, 42.42.42.42424), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.11 (10.0.11, 42.42.42.42424), X64 RyuJIT x86-64-v3


```
| Method           | Count    | Mean     | Error     | StdDev    | Median   | Gen0     | Gen1     | Gen2     | Allocated  |
|----------------- |--------- |---------:|----------:|----------:|---------:|---------:|---------:|---------:|-----------:|
| Array_Read       | 10000000 | 5.453 ms | 0.1140 ms | 0.3363 ms | 5.270 ms | 390.6250 | 390.6250 | 390.6250 | 40000146 B |
| UnsafeArray_Read | 10000000 | 7.818 ms | 0.0803 ms | 0.0670 ms | 7.804 ms |        - |        - |        - |          - |
