public class IngredientDto2
{
    public int? Id { get; set; }
    public required string Name { get; set; }
    public List<RecipeDto2>? Recipes { get; set; }
}