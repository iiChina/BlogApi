using Blog.Data;
using Blog.Models;
using BlogApi.Extensions;
using BlogApi.Services;
using BlogApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace BlogApi.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        /*
            [FromServices] 

                =
                
            private readonly TokenService _tokenService;
            public AccountController(TokenService tokenService)
            {
                _tokenService = tokenService;
            }
        */

        [HttpPost("v1/accounts")]
        public async Task<IActionResult> Post(
            [FromBody]RegisterViewModel model,
            [FromServices]BlogDataContext context 
        )
        {
            if(ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };

            var password = PasswordGenerator.Generate(25);

            user.PasswordHash = PasswordHasher.Hash(password);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<dynamic>(new 
                {
                    user = user.Email,
                    password
                }));
            }
            catch(DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna do servidor"));
            }
        }

        [HttpPost("v1/accounts/login")]
        //Criamos uma dependencia da classe tokenService nesse método/controller
        public async Task<IActionResult> Login(
            [FromBody]LoginViewModel model,
            [FromServices]BlogDataContext context,
            [FromServices]TokenService tokenService)
        {
            if(!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
            
            var user = await context
                .Users
                .AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if(user == null)
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

            if(!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
            }
            
        }
    }
}