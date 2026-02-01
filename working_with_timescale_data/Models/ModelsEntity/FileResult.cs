namespace working_with_timescale_data.Models.ModelsEntity;

public class FileResultEntity
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public DateTime FirstoperationTime { get; set; }
    public double TimeDeltaSeconds { get; set; }
    public double AvgExecutionTime { get; set; }
    public double AvgValue { get; set; }
    public double MedianValue { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; } 
}
