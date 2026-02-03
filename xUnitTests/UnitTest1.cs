using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Reflection;
using working_with_timescale_data.Controllers;
using working_with_timescale_data.Models.DbContextDir;
namespace xUnitTests;

public class MeasurementsControllerUnitTest
{
    private MeasurementsController _controller;
    private TimescaleDb _dbContext;
    private Mock<IFormFile> _mockFile;
    public MeasurementsControllerUnitTest()
    {
        Initialization();
    }

    private void Initialization()
    {
        var optionForDb = new DbContextOptionsBuilder<TimescaleDb>();
        optionForDb.UseNpgsql("Host=localhost;Port=5432;Database=TimescaleDb;Username=postgres;Password=42924870");
        _dbContext = new TimescaleDb(optionForDb.Options);
        _controller = new MeasurementsController(_dbContext);
        _mockFile = new Mock<IFormFile>();
    }

    [Fact]
    public void CalculateMedian_test()
    {
        var values = new List<double>(){ 1.0, 2.0, 3.0, 4.0 };

        var controller = new MeasurementsController(null);
        var methodcalCalculateMedian = typeof(MeasurementsController).
                                       GetMethod("CalculateMedian", BindingFlags.NonPublic | BindingFlags.Instance);

        var result = Convert.ToDouble(methodcalCalculateMedian.Invoke(controller,new object[] {values}));
        Console.WriteLine("Test log");
        Assert.Equal(3.5, result);
    }
}
