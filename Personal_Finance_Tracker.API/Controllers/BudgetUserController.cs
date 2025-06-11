using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Finance_Tracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetUserController : ControllerBase
    {
        private readonly IBudgetUser _budgetUser;
        private readonly ILogger<BudgetUserController> _logger;

        public BudgetUserController(IBudgetUser budgetUser, ILogger<BudgetUserController> logger)
        {
            _budgetUser = budgetUser;
            _logger = logger;
        }

        [HttpPost("{idBudget}")]
        public async Task<IActionResult> AddToBudget([FromRoute] int idBudget, [FromBody] string email)
        {
            try
            {
                var res = await _budgetUser.AddUserToBudget(User, email, idBudget);
                return res.IsSuccess ? Ok(res) : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error adding user to budget ID {idBudget} with email {email}", idBudget, email);
                return BadRequest();
            }
        }

        [HttpDelete("{idBudget}")]
        public async Task<IActionResult> RemFromBudget([FromRoute] int idBudget, [FromBody] string email)
        {
            try
            {
                var res = await _budgetUser.RemoveUserFromBudget(User, email, idBudget);
                return res.IsSuccess ? Ok(res) : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error removing user from budget ID {idBudget} with email {email}", idBudget, email);
                return BadRequest();
            }
        }

        [HttpGet("{idBudget}")]
        public async Task<IActionResult> SubscribesBudget([FromRoute] int idBudget)
        {
            try
            {
                var res = await _budgetUser.UsersSubscribeInAdminBudget(User, idBudget);
                return res.IsSuccess ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting subscribed users for budget ID {idBudget}", idBudget);
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> BudgetsAsWatcher()
        {
            try
            {
                var res = await _budgetUser.MyBudgetsAsWatcher(User);
                return res.IsSuccess ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting budgets as watcher");
                return BadRequest();
            }
        }
    }
}
