namespace working_with_timescale_data.Controllers;

[Controller]
public class MeasurementsController : Controller
{
    private readonly TimescaleDb _context;
    public MeasurementsController(TimescaleDb context)
    {
        _context = context;
    }

    [HttpGet("Tttt")]
    public async Task<IActionResult> Test()
    { 
        
        return default;
    }


    [HttpPost]
    public async Task<IActionResult> UploadCsv(IFormFile form)
    {
        using(IDbContextTransaction contextTransaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                List<CsvRow> rows = await ParseCsvAsync(form);
                ValidateRows(rows);

                var fileName = Path.GetFileNameWithoutExtension(form.FileName);
                await DeleteExistingData(fileName);

                var newMeasurements = rows.Select(r => new MeasurementEntity
                {
                    FileName = fileName,
                    Date = r.Date,
                    ExecutionTime = r.ExecutionTime,
                    Value = r.Value
                }).ToList();

                FileResultEntity fileResult = await CalculatinReasultsAndSave(newMeasurements, fileName, rows);

                contextTransaction.Commit();
                return Ok(fileResult);
            }
            catch(Exception ex)
            {
                await contextTransaction.RollbackAsync();
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetResults(
                                     string? fileName = null,
                                     DateTime? startFrom = null,
                                     DateTime? startto = null,
                                     double? avgValueFrom = null,
                                     double? avgValueTo = null,
                                     double? avgTimeFrom = null,
                                     double? avgTimeTo = null
                                                           )
    {
        var query = _context.FileResults.AsQueryable();

        if(!string.IsNullOrEmpty(fileName))
            query = query.Where(x => x.FileName == fileName);
        //

        if(startFrom.HasValue)
            query = query.Where(x => x.FirstoperationTime >= startFrom.Value);

        if(startto.HasValue)
            query = query.Where(x => x.FirstoperationTime <= startto.Value);
        //

        if(avgValueFrom.HasValue)
            query = query.Where(x => x.AvgValue >= avgValueFrom.Value);

        if(avgValueTo.HasValue)
            query = query.Where(x => x.AvgValue <= avgValueTo.Value);
        //

        if(avgTimeFrom.HasValue)
            query = query.Where(x => x.AvgExecutionTime >= avgTimeFrom.Value);

        if(avgTimeTo.HasValue)
            query = query.Where(x => x.AvgExecutionTime <= avgTimeTo.Value);
        //

        var results = await query.ToListAsync();
        return Ok(results);
    }
    [HttpGet]
    public async Task<IActionResult> GetLatest(string fileName)
    {
        var measurments = await _context.Measurements
                                .Where(m=>m.FileName == fileName)
                                .OrderByDescending(m=>m.Date)
                                .Take(10)
                                .Select(m => new {m.Date,m.ExecutionTime,m.Value})
                                .ToListAsync();
        return Ok(measurments);
    }


    private async Task<FileResultEntity> CalculatinReasultsAndSave(List<MeasurementEntity> newMeasurements, string fileName, List<CsvRow> rows)
    {
        await _context.Measurements.AddRangeAsync(newMeasurements);

        var result = CalculateResults(fileName, rows);

        await _context.FileResults.AddAsync(result);
        await _context.SaveChangesAsync();

        return result;
    }

    private FileResultEntity CalculateResults(string fileName, List<CsvRow> rows)
    {
        var dates = rows.Select(r => r.Date);
        var values = rows.Select(r => r.Value).ToList();
        var medianValue = CalculateMedian(values);

        return new FileResultEntity
        {
            FileName = fileName,
            FirstoperationTime = dates.Min(),
            TimeDeltaSeconds = (dates.Max() - dates.Min()).TotalSeconds,
            AvgExecutionTime = rows.Average(r => r.ExecutionTime),
            AvgValue = values.Average(),
            MedianValue = medianValue,
            MaxValue = values.Max(),
            MinValue = values.Min(),

        };

    }

    private double CalculateMedian(List<double> values)
    {
        values.Sort();
        Int32 mid = values.Count / 2;

        return values.Count % 2 == 0 ? (values[mid - 1] + values[mid] / 2) : values[mid];
    }

    private async Task DeleteExistingData(string fileName)
    {
        var oldMeasurements = await _context.Measurements.
                                             Where(m => m.FileName == fileName).
                                             ToListAsync();
        var oldResult = await _context.FileResults.
                                       FirstOrDefaultAsync(f => f.FileName == fileName);

        if(oldMeasurements.Any())
            _context.Measurements.RemoveRange(oldMeasurements);

        if(oldResult is not null)
            _context.FileResults.Remove(oldResult);
    }

    private void ValidateRows(List<CsvRow> rows)
    {
        if(rows.Count < 1 || rows.Count > 10000)
            throw new Exception("Не допустимое колличство строк");

        foreach(CsvRow row in rows)
        {
            if(row.Date < new DateTime(2000, 1, 1) || row.Date > DateTime.UtcNow)
                throw new Exception("Дата не корpектная  ");

            if(row.ExecutionTime < 0)
                throw new Exception("Не допустимое время выполнения : Не может быть отрицательным ");

            if(row.Value < 0)
                throw new Exception("Не допустимое значение : Не может быть отрицательным ");
        }
    }

    private async Task<List<CsvRow>> ParseCsvAsync(IFormFile file)
    {
        var rows = new List<CsvRow>();
        using(var reader = new StreamReader(file.OpenReadStream()))
        {
            await reader.ReadLineAsync();

            while(!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if(string.IsNullOrEmpty(line))
                    continue;

                var parts = line.Split(';');
                if(parts.Length != 3)
                    continue;

                rows.Add(new CsvRow
                {
                    Date = DateTime.Parse(parts[0]),
                    ExecutionTime = double.Parse(parts[1]),
                    Value = double.Parse(parts[2])
                });
            }
        }
        return rows;
    }



    private class CsvRow
    {
        public DateTime Date { get; set; }
        public double ExecutionTime { get; set; }
        public double Value { get; set; }
    }
}
