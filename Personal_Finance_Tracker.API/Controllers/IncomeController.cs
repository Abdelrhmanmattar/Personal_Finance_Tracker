using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Finance_Tracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IncomeController : ControllerBase
    {
        private readonly IIncome incomeDB;
        private readonly ILogger<IncomeController> _logger;

        public IncomeController(IIncome _incomeDB, ILogger<IncomeController> logger)
        {
            incomeDB = _incomeDB;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> AddIncome([FromBody] IncomeDTO dTO)
        {
            try
            {
                var res = await incomeDB.Create(User, dTO);
                return res.IsSuccess
                    ? CreatedAtAction(nameof(GetIncome), new { ID = res.Data?.Id }, res.Data)
                    : BadRequest(res);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error adding income with Id {Id}", dTO?.Id);
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<IActionResult> AllIncome()
        {
            try
            {
                var res = await incomeDB.GetAll(User);
                return res.IsSuccess ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error retrieving all income");
                return BadRequest();
            }
        }
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetIncome([FromRoute] int ID)
        {
            try
            {
                var res = await incomeDB.GetById(User, ID);
                return res.IsSuccess ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error retrieving income with ID {ID}", ID);
                return BadRequest();
            }
        }
        [HttpDelete("{ID}")]
        public async Task<IActionResult> DelIncome([FromRoute] int ID)
        {
            try
            {
                var res = await incomeDB.Remove(User, ID);
                return res.IsSuccess ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error deleting income with ID {ID}", ID);
                return BadRequest();
            }
        }
        [HttpPut("{ID}")]
        public async Task<IActionResult> UpdateIncome([FromRoute] int ID, [FromBody] IncomeDTO dTO)
        {
            try
            {
                var res = await incomeDB.Update(User, ID, dTO);
                return res.IsSuccess ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error updating income with ID {ID}", ID);
                return BadRequest();
            }
        }
        [HttpGet("date")]
        public async Task<IActionResult> FilterByDate([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var res = await incomeDB.GetByDateRange(User, fromDate, toDate);
                return Ok(res.Data);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error filtering income from {FromDate} to {ToDate}", fromDate, toDate);
                return BadRequest();
            }
        }
    }
}
