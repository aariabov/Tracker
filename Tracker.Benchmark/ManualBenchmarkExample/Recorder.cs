using System.Diagnostics;
using BenchmarkDotNet.Toolchains.Parameters;
using static System.Console;
using static System.Diagnostics.Process;

namespace Tracker.Benchmark;

public static class Recorder
{
    private static readonly Stopwatch _timer = new();
    private static long _bytesPhysicalBefore = 0;
    private static long _bytesVirtualBefore = 0;

    public static void Start()
    {
        // инициируем очистку памяти сборщиками мусора
        // очищается память, на которую никто не ссылается
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        // храним текущие затраты физической и виртуальной памяти
        _bytesPhysicalBefore = GetCurrentProcess().WorkingSet64;
        _bytesVirtualBefore = GetCurrentProcess().VirtualMemorySize64;
        _timer.Restart();
    }

    public static void Stop()
    {
        _timer.Stop();
        var bytesPhysicalAfter = GetCurrentProcess().WorkingSet64;
        var bytesVirtualAfter = GetCurrentProcess().VirtualMemorySize64;
        WriteLine("{0:N0} physical bytes used.", bytesPhysicalAfter - _bytesPhysicalBefore);
        WriteLine("{0:N0} virtual bytes used.", bytesVirtualAfter - _bytesVirtualBefore);
        WriteLine("{0} time span elapsed.", _timer.Elapsed);
        WriteLine("{0:N0} total milliseconds elapsed.", _timer.ElapsedMilliseconds);
    }

    public static void Execute(string methodName, Action action)
    {
        WriteLine($"\nBenchmark method {methodName}");
        Start();
        action();
        Stop();
    }
}
