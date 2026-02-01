using Microsoft.AspNetCore.Mvc;
using working_with_timescale_data.Models.DbContextDir;
namespace working_with_timescale_data.Controllers
{
    [Controller]
    public class MeasurementsController : Controller
    {
        private readonly TimescaleDb _context;
        public MeasurementsController(TimescaleDb context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCsv()
        {

        }



    }

}
