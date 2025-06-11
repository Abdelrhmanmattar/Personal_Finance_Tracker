using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Finance_Tracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpense expenseDB;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(IExpense _expenseDB, ILogger<ExpenseController> logger)
        {
            expenseDB = _expenseDB;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> AddExpen([FromBody] ExpensesDTO dTO)
        {
            try
            {
                var res = await expenseDB.AddExpense(dTO, User);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error adding expense with Id {Id}", dTO?.Id);
                return BadRequest();
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var res = await expenseDB.GetAll(User);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting all expenses");
                return BadRequest();
            }
        }

        [HttpGet("{ID}")]
        public async Task<IActionResult> Get([FromRoute] int ID)
        {
            try
            {
                var res = await expenseDB.GetById(User, ID);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting expense with ID {ID}", ID);
                return BadRequest();
            }
        }


        [HttpPut]
        public async Task<IActionResult> EditExpen([FromBody] ExpensesDTO dTO)
        {
            try
            {
                var res = await expenseDB.Update(User, dTO);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error editing expense with ID {ID}", dTO?.Id);
                return BadRequest();
            }
        }

        [HttpDelete("{ExpenID}")]
        public async Task<IActionResult> DelExpen([FromRoute] int ExpenID)
        {
            try
            {
                var res = await expenseDB.Remove(User, ExpenID);
                return res.IsSuccess ? Ok(res) : BadRequest(res);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error deleting expense with ID {ID}", ExpenID);
                return BadRequest();
            }
        }
        [HttpGet("total")]
        public IActionResult getTotalCategory([FromQuery] int Cat_Id)
        {
            try
            {
                var res = expenseDB.GetTotalExpenses(User, Cat_Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting total expenses for category ID {Cat_Id}", Cat_Id);
                return BadRequest();
            }
        }
        [HttpGet("date")]
        public IActionResult FilterByDate([FromQuery] int CatID, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var res = expenseDB.GetByDateRange(User, CatID, fromDate, toDate);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error filtering expenses for category ID {CatID} from {fromDate} to {toDate}", CatID, fromDate, toDate);
                return BadRequest();
            }
        }
    }
}
