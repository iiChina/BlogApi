using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("v1/login")]
        public IActionResult Login()
        {
            var tokenService = new TokenService();
            var token = tokenService.GenerateToken(null);
            
            return Ok(token);
        }
    }
}