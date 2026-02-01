namespace working_with_timescale_data.Models.ModelsEntity;

public class MeasurementEntity
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public DateTime Date { get; set; }
    public double ExecutionTime { get; set; }
    public double Value     { get; set; }
}

