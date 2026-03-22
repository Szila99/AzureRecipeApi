public interface IRecipeService
{
    Task<List<RecipeDto1>> GetRecipes(string? name, string? ingredientName, string? descriptionKeyword, string? image);
    Task<List<RecipeDto1>> GetFullyCookableRecipes(List<string> ingredients);
    Task AddRecipe(RecipeDto1 recipes);
    Task UpdateRecipe(RecipeDto1 updtRecipe);
    Task DeleteRecipe(int recipeId);
}