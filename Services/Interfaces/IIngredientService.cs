public interface IIngredientService
{
    Task<List<IngredientDto2>> GetIngredients(string? name, string? recipe);
    Task AddIngredient(IngredientDto2 ingredient);

    Task UpdateIngredient(IngredientDto1 updtIngredient);

    Task DeleteIngredient(int IngredientId);
}