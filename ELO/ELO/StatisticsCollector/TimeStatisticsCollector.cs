namespace ELO.StatisticsCollector;

public class TimeStatisticsCollector(string outputFilePath) : StatisticsCollector
{
    public override void RunAndMeasure(CodeToMeasure code)
    {
        var duration = MeasureExecutionTime(code);

        using var writer = new StreamWriter(outputFilePath, true);
        
        writer.WriteLine($"{duration.Ticks}");
    }
}