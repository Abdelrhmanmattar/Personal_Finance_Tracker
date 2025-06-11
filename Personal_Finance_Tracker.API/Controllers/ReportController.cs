using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Finance_Tracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReport _report;

        public ReportController(IReport report)
        {
            this._report = report;
        }
        [HttpGet("summary")]
        public IActionResult Getsummary()
        {
            var res = _report.Summary(User);
            return Ok(res);
        }
    }
}
