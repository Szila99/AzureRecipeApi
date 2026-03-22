using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : Controller
{
    private readonly IRecipeService _service;

    public RecipeController(IRecipeService recipeService)
    {
        _service = recipeService;
    }
    public static List<Recipes> recipeList = new List<Recipes>();

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeDto1>>> GetRecipes([FromQuery] string? name, [FromQuery] string? ingredientName, [FromQuery] string? descriptionKeyword, [FromQuery] string? image)
    {
        try
        {
            var recipes = await _service.GetRecipes(name, ingredientName, descriptionKeyword, image);
            return Ok(recipes);
        }
        catch (KeyNotFoundException)
        {
            return NoContent();
        }
    }

    [HttpGet("Cookable")]
    public async Task<ActionResult<IEnumerable<RecipeDto1>>> GetFullyCookableRecipes ([FromQuery] List<string> ingredients)
    {
        try
        {
            var recipes = await _service.GetFullyCookableRecipes(ingredients);
            return Ok(recipes);
        }
        catch (ArgumentNullException argnex)
        {
            return BadRequest(argnex.Message);
        }
        catch (KeyNotFoundException kex)
        {
            return NotFound(kex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddRecipe([FromBody] RecipeDto1 newRecipeDto)
    {
        try
        {
            await _service.AddRecipe(newRecipeDto);
            return Created();
        }
        catch (InvalidOperationException iopex)
        {
            return BadRequest(iopex.Message);
        }
        catch (ArgumentNullException argnex)
        {
            return BadRequest(argnex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult> UpdateRecipe([FromBody] RecipeDto1 updtRecipe)
    {
        try
        {
            await _service.UpdateRecipe(updtRecipe);
            return Ok("Recipe succesfully updated!");
        }
        catch (KeyNotFoundException kex)
        {
            return BadRequest(kex.Message);
        }
        catch (InvalidOperationException iopex)
        {
            return BadRequest(iopex.Message);
        }

    }

    [HttpDelete]
    public async Task<ActionResult> DeleteRecipe([FromQuery] int recipeId)
    {
        try
        {
            await _service.DeleteRecipe(recipeId);
            return NoContent();
        }
        catch (KeyNotFoundException kex)
        {
            return NotFound(kex.Message);
        }
    }
}