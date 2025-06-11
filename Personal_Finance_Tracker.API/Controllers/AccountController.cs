using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Finance_Tracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMainUser_Repo _mainUser_Repo;

        public AccountController(IMainUser_Repo mainUser_Repo)
        {
            _mainUser_Repo = mainUser_Repo;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LogIn_DTO dTO)
        {
            try
            {
                var res = await _mainUser_Repo.LogIn_User(dTO);
                return (res.IsSuccess) ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception x)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(x.Message);
                Console.ResetColor();
                return BadRequest();
            }
        }
        [HttpPost("Signup")]
        public async Task<IActionResult> Signup(Regisiter_DTO dTO)
        {
            try
            {
                var res = await _mainUser_Repo.Regisiter_User(dTO);
                return (res.IsSuccess) ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception x)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(x.Message);
                Console.ResetColor();
                return BadRequest();
            }

        }
    }
}
