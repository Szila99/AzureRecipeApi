public interface IRecipeRepository
{
    IQueryable<Recipes> GetAllRecipes();
    IQueryable<Ingredient> GetAllIngredients();
    Task AddRecipe(Recipes recipe);
    Task AddIngredient(Ingredient ingredient);
    Task UpdateRecipe(Recipes recipe);
    Task DeleteRecipe(Recipes recipe);
}