using Blog.Data;
using Blog.Models;
using BlogApi.Extensions;
using BlogApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        //1- Uma convenção é mapear as Actions dessas controllers utilizando o nome da controller no plural na URL. 
        //2- Outra convenção é versionar os métodos da API para ter retrocompatibilidade.
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context) 
        {
            try
            {
                var categories = await context.Categories.ToListAsync();
                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05XE05 Falha interna do servidor"));
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context) 
        {
            try
            {
                var category = await context.Categories
                    .FirstOrDefaultAsync(x => x.Id == id);

                if(category == null)
                    return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Category>(category));
            }
             catch(Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE04 Falha interna do servidor"));
            }
            
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context
        )
        {
            if(ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try
            {
                var category = new Category
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug.ToLower()
                };

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch(DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE9 Não foi possível incluir a categoria"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE10 Falha interna do servidor"))   ;
            }

        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context
        )
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if(category == null)
                    return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

                category.Name = model.Name;
                category.Slug = model.Slug.ToLower();

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch(DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE8 Não foi possível alterar a categoria"));
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE11 Falha interna do servidor"));
            }
            
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            int id, 
            [FromServices] BlogDataContext context
        )
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if(category == null)
                    return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch(DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE7 Não foi possível deletar a categoria"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE12 Falha interna do servidor"));
            }
        }
    }
}