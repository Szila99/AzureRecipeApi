public class Recipes
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Image { get; set; }

    public required List<Ingredient> Ingredient { get; set; } = new List<Ingredient>();
}