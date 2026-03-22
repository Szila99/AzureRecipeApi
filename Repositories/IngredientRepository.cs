using Recipe;
using Microsoft.EntityFrameworkCore;

public class IngredientRepository : IIngredientRepository
{
    private readonly RecipeContext _context;

    public IngredientRepository(RecipeContext context)
    {
        _context = context;
    }

    public IQueryable<Ingredient> GetAllIngredients()
    {
        return _context.Ingredients.Include(i => i.Recipes);
    }

    //Azert kell, hogy ne a repoban vegeznem a business logikat
    public IQueryable<Recipes> GetAllRecipes()
    {
        return _context.Recipes;
    }

    public async Task AddIngredient(Ingredient ingredient)
    {
        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateIngredient(Ingredient ingredient)
    {
        _context.Ingredients.Update(ingredient);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteIngredient(Ingredient ingredient)
    {
        //Osszes recept ami tartalmazza az ingredientunket
        var recipesWithIngredient = await _context.Recipes
                                                .Where(r => r.Ingredient
                                                .Any(i => i.Id == ingredient.Id))
                                                .ToListAsync();
        //Torli a receptekbol
        foreach (var recipe in recipesWithIngredient)
        {
            recipe.Ingredient.Remove(ingredient);
        }

        _context.Ingredients.Remove(ingredient);
        await _context.SaveChangesAsync();
    }
}