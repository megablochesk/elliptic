using System.Diagnostics;

namespace ELO.StatisticsCollector;

public abstract class StatisticsCollector
{
    public delegate void CodeToMeasure();

    public abstract void RunAndMeasure(CodeToMeasure code);

    protected TimeSpan MeasureExecutionTime(CodeToMeasure code)
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        code();
        stopwatch.Stop();

        return stopwatch.Elapsed;
    }
}