using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Finance_Tracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategory categoryDB;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategory _category, ILogger<CategoryController> logger)
        {
            categoryDB = _category;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDTO dTO)
        {
            try
            {
                var res = await categoryDB.Add(User, dTO);
                return res.IsSuccess ? Created() : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error adding category with name: {name}", dTO?.Name);
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetCatss()
        {
            try
            {
                var res = await categoryDB.GetAll(User);
                return res.IsSuccess ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting categories");
                return BadRequest();
            }
        }
        [HttpDelete("{ID}")]
        public async Task<IActionResult> RemCategory([FromRoute] int ID)
        {
            try
            {
                var res = await categoryDB.Remove(User, ID);
                return res.IsSuccess ? Ok(res.Data) : BadRequest(res.ErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error removing category with ID: {ID}", ID);
                return BadRequest();
            }
        }
    }
}
