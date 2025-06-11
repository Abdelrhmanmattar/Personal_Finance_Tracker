using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Finance_Tracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudget budgetDB;
        private readonly ILogger<BudgetController> _logger;

        public BudgetController(IBudget _budgetDB, ILogger<BudgetController> logger)
        {
            _logger = logger;
            budgetDB = _budgetDB;
        }
        [HttpPost]
        public async Task<IActionResult> ADDBUD(BudgetDTO dTO)
        {
            try
            {
                var res = await budgetDB.Create(User, dTO);
                return (res.IsSuccess) ? CreatedAtAction(nameof(GetID), new { ID = res.Data?.ID }, res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception x)
            {
                _logger.LogWarning(x.Message);
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var values = await budgetDB.GetAll(User);
                return (values.IsSuccess) ? Ok(values.Data) : BadRequest(values?.ErrorMessage);
            }
            catch (Exception x)
            {
                _logger.LogWarning(x.Message);
                return BadRequest();
            }
        }
        [HttpGet("{ID}")]
        public async Task<IActionResult> GetID([FromRoute] int ID)
        {
            try
            {
                var res = await budgetDB.GetById(User, ID);
                return (res.IsSuccess) ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception x)
            {
                _logger.LogWarning(x.Message);
                return BadRequest();
            }
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BudgetDTO budget)
        {
            try
            {
                var res = await budgetDB.Update(User, budget);
                return (res.IsSuccess) ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception x)
            {
                _logger.LogWarning(x.Message);
                return BadRequest();
            }
        }
        [HttpDelete("{ID}")]
        public async Task<IActionResult> Delete([FromRoute] int ID)
        {
            try
            {
                var res = await budgetDB.Remove(User, ID);
                return (res.IsSuccess) ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception x)
            {
                _logger.LogWarning(x.Message);
                return BadRequest();
            }
        }
    }
}
