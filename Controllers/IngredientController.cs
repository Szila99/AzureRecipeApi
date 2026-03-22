using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IngredientController : Controller
{
    private readonly IIngredientService _service;

    public IngredientController(IIngredientService ingredientService)
    {
        _service = ingredientService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredientDto2>>> GetIngredients([FromQuery] string? name, [FromQuery] string? recipe)
    {
        try
        {
            var ingredients = await _service.GetIngredients(name, recipe);
            return Ok(ingredients);
        }
        catch (KeyNotFoundException)
        {
            return NoContent();
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddIngredient([FromBody] IngredientDto2 newIngredientDto)
    {
        try
        {
            await _service.AddIngredient(newIngredientDto);
            return Created();
        }
        catch (ArgumentNullException argnex)
        {
            return BadRequest(argnex.Message);
        }
        catch (ArgumentException argex)
        {
            return Conflict(argex.Message);
        }
        catch (KeyNotFoundException kex)
        {
            return NotFound(kex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult> UpdateIngredient([FromBody] IngredientDto1 updtIngredient)
    {
        try
        {
            await _service.UpdateIngredient(updtIngredient);
            return Ok("Ingredient succesfully updated!");
        }
        catch (KeyNotFoundException kex)
        {
            return NotFound(kex.Message);
        }
        catch (ArgumentException argex)
        {
            return BadRequest(argex.Message);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteIngredient([FromQuery] int IngredientId)
    {
        try
        {
            await _service.DeleteIngredient(IngredientId);
            return NoContent();
        }
        catch (KeyNotFoundException kex)
        {
            return NotFound(kex.Message);
        }
    }
}