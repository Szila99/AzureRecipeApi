public class Ingredient
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required List<Recipes> Recipes { get; set; } = new List<Recipes>();
}