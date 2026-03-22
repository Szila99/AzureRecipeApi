using Microsoft.EntityFrameworkCore;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _repository;

    public IngredientService(IIngredientRepository ingredientRepository)
    {
        _repository = ingredientRepository;
    }

    public async Task<List<IngredientDto2>> GetIngredients(string? name, string? recipe)
    {
        var repo = _repository.GetAllIngredients();

        //Nincsenek ingredientek
        if (repo == null || !repo.Any())
        {
            throw new KeyNotFoundException("No ingredient found");
        }

        //Nev szerint szur
        if (!string.IsNullOrEmpty(name))
        {
            repo = repo.Where(i => i.Name.ToLower().Contains(name.ToLower()));
        }

        //Recept szerint szur
        if (!string.IsNullOrEmpty(recipe))
        {
            repo = repo.Where(i => i.Recipes.Any(r => r.Name.ToLower().Contains(recipe.ToLower())));
        }

        //Vissza teriti az ingredienteket receptestol
        return await repo.Select(i => new IngredientDto2
        {
            Id = i.Id,
            Name = i.Name.ToLower(),
            Recipes = i.Recipes.Select(r => new RecipeDto2
            {
                Id = r.Id,
                Name = r.Name.ToLower()
            }).ToList()
        }).ToListAsync();
    }

    public async Task AddIngredient(IngredientDto2 ingredient)
    {
        //Teszteli, hogy null e
        if (ingredient == null)
        {
            throw new ArgumentNullException("Ingredient can`t be null!");
        }

        //Nzi, hogy letezik e mar az ingredient(nev szerint azert, hogy ne lehessen ket egyforma hozzavalo)
        if (_repository.GetAllIngredients().Any(i => i.Name.ToLower() == ingredient.Name.ToLower()))
        {
            throw new ArgumentException("Wrong Name for ingredient, allready exists in the table!");
        }

        //Letrehozzuk az ingredientet
        var existingIngredient = new Ingredient
        {
            Name = ingredient.Name.ToLower(),
            Recipes = new List<Recipes>()
        };

        //Lekerjuk az osszes receptet
        var allRecipes = _repository.GetAllRecipes();

        //Ha vannak receptek megadva es azok mar leteznek az adatbazisban akkor azokat hozza adj az ingredient recept listajahoz
        if (ingredient.Recipes != null)
        {
            foreach (var recipeDto in ingredient.Recipes)
            {
                var recipe = allRecipes.FirstOrDefault(r => r.Id == recipeDto.Id);

                //Ha nem letezik a recept az hiba
                if (recipe == null)
                {
                    throw new KeyNotFoundException("Recipe can`t be found in the database!");
                }
                else
                {
                    existingIngredient.Recipes.Add(recipe);
                }
            }
        }

        await _repository.AddIngredient(existingIngredient);
    }

    public async Task UpdateIngredient(IngredientDto1 updtIngredient)
    {
        //Megkeresi az ingredientet
        var ingredient = await _repository.GetAllIngredients().FirstOrDefaultAsync(i => i.Id == updtIngredient.Id);

        //Ha az ingredient null akkor nem lehet frissiteni
        if (await _repository.GetAllIngredients().FirstOrDefaultAsync(i => i.Name.ToLower() == updtIngredient.Name.ToLower()) != null)
        {
            throw new ArgumentException("Wrong Name for ingredient, it allready exists!");
        }

        if (ingredient == null)
        {
            throw new KeyNotFoundException("Id can`t be found in the database!");
        }

        ingredient.Name = updtIngredient.Name;

        await _repository.UpdateIngredient(ingredient);
    }

    public async Task DeleteIngredient(int IngredientId)
    {
        var ingredient = _repository.GetAllIngredients().FirstOrDefault(i => i.Id == IngredientId);

        //Ha nincs ilyen akkor nem is lehet torolni
        if (ingredient == null)
        {
            throw new KeyNotFoundException("Id can`t be found in Ingredient table!");
        }

        await _repository.DeleteIngredient(ingredient);
    }

}