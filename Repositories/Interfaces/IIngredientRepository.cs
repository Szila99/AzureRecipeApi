public interface IIngredientRepository
{
    IQueryable<Ingredient> GetAllIngredients();
    IQueryable<Recipes> GetAllRecipes();
    Task AddIngredient(Ingredient ingredient);
    Task UpdateIngredient(Ingredient ingredient);
    Task DeleteIngredient(Ingredient ingredient);
}