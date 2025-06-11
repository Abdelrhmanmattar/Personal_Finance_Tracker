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
        public async Task<IActionResult> Getsummary([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var res = await _report.Summary(User, from, to);
            return Ok(res);
        }
    }
}
