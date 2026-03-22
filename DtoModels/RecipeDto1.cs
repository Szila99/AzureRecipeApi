public class RecipeDto1
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Image { get; set; }
    public required List<IngredientDto1> Ingredient { get; set; }
}