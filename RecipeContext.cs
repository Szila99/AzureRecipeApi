using Microsoft.EntityFrameworkCore;

namespace Recipe
{
    public class RecipeContext : DbContext
    {
        public RecipeContext(DbContextOptions<RecipeContext> options) : base(options)
        {
        }
        public DbSet<Recipes> Recipes { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }
    }
}