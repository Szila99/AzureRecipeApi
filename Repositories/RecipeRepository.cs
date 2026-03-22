using Recipe;
using Microsoft.EntityFrameworkCore;

public class RecipeRepository : IRecipeRepository
{
    private readonly RecipeContext _context;

    public RecipeRepository(RecipeContext context)
    {
        _context = context;
    }

    public IQueryable<Recipes> GetAllRecipes()
    {
        return _context.Recipes
            .Include(r => r.Ingredient);
    }

    public IQueryable<Ingredient> GetAllIngredients()
    {
        return _context.Ingredients;
    }

    public async Task AddRecipe(Recipes recipe)
    {
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
    }

    public async Task AddIngredient(Ingredient ingredient)
    {
        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRecipe(Recipes recipe)
    {
        _context.Recipes.Update(recipe);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRecipe(Recipes recipe)
    {
        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
    }
}