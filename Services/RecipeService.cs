
using Microsoft.EntityFrameworkCore;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _repository = recipeRepository;
    }

    public async Task<List<RecipeDto1>> GetRecipes(string? name, string? ingredientName, string? descriptionKeyword, string? image)
    {
        var repo = _repository.GetAllRecipes();

        //Szures nev szerint
        if (!string.IsNullOrEmpty(name))
        {
            repo = repo.Where(r => r.Name.ToLower().Contains(name.ToLower()));
        }

        //Szures hozzavalo szerint
        if (!string.IsNullOrEmpty(ingredientName))
        {
            repo = repo.Where(r => r.Ingredient.Any(i => i.Name.ToLower().Contains(ingredientName.ToLower())));
        }

        //Szures a leiras szerint
        if (!string.IsNullOrEmpty(descriptionKeyword))
        {
            repo = repo.Where(r => r.Description.ToLower().Contains(descriptionKeyword.ToLower()));
        }

        if (!string.IsNullOrEmpty(image))
        {
            repo = repo.Where(r => r.Image.Contains(image));
        }

        //Ures a lista
        if (repo == null || !repo.Any())
        {
            throw new KeyNotFoundException("No recipe found");
        }

        //Visszaadja a recepteket az ingredientekkel egyutt
        return await repo.Select(r => new RecipeDto1
        {
            Id = r.Id,
            Name = r.Name.ToLower(),
            Description = r.Description.ToLower(),
            Image = r.Image,
            Ingredient = r.Ingredient.Select(i => new IngredientDto1
            {
                Id = i.Id,
                Name = i.Name.ToLower()
            }).ToList()
        }).ToListAsync();

    }

    //Sajat metodus, azt csinalja, hogy vissza ad minden receptet amit eltudunk kesziteni a megadott ingredientjeinkbol
    public async Task<List<RecipeDto1>> GetFullyCookableRecipes(List<string> ingredients)
    {
        //Ha nincs ingredient akkor fozni sem lehet belole
        if (!ingredients.Any())
        {
            throw new ArgumentNullException("You don`t have any ingredient!");
        }

        //Osszes recept
        var recipes = await _repository.GetAllRecipes().ToListAsync();

        ingredients = ingredients.Select(i => i.ToLower()).ToList();

        //Elkeszitheto receptek listaja
        var cookableRecipes = new List<RecipeDto1>();

        foreach (var recipe in recipes)
        {
            bool canCook = recipe.Ingredient.All(i => ingredients.Contains(i.Name.ToLower()));

            //Recepes-bol RecipeDto1-be
            var goodRecipe = new RecipeDto1
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                Image = recipe.Image,
                Ingredient = new List<IngredientDto1>()
            };
            
            //Az ingredientek kiirasahoz kell, hogy tiszta legyen, hogy mit is kell hasznaljunk a recepthez
            foreach (var ingredient in recipe.Ingredient)
            {
                var ingredientDto = new IngredientDto1
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name
                };
                goodRecipe.Ingredient.Add(ingredientDto);
            }

            //Hozzaadjuk a receptet a elkeszitheto receptekhez
            if (canCook)
            {
                cookableRecipes.Add(goodRecipe);
            }

        }

        //Ha neincs elkeszitheto akkor exception-t dobunk
        if (!cookableRecipes.Any())
        {
            throw new KeyNotFoundException("Can`t find recipe!");
        }

        return cookableRecipes;
    }

    public async Task AddRecipe(RecipeDto1 recipeDto)
    {
        //Teszt, hogy null e
        if (recipeDto == null)
        {
            throw new ArgumentNullException("Recipe can`t be null!");
        }

        //Letezik e mar az Id
        if (_repository.GetAllIngredients().FirstOrDefault(i => i.Id == recipeDto.Id) != null || recipeDto.Id < 0)
        {
            throw new InvalidOperationException("Wrong recipe Id!");
        }

        //Hozzavalo nelkul nincs mit fozni
        if (recipeDto.Ingredient == null || !recipeDto.Ingredient.Any())
        {
            throw new InvalidOperationException("You can`t make a recipe without ingredient!");
        }

        //Uj recept
        var recipe = new Recipes
        {
            Name = recipeDto.Name.ToLower(),
            Description = recipeDto.Description.ToLower(),
            Image = recipeDto.Image,
            Ingredient = new List<Ingredient>()
        };

        //Osszes ingredient
        var allIngredients = _repository.GetAllIngredients();

        foreach (var ingredientDto in recipeDto.Ingredient)
        {
            var ingredient = allIngredients.FirstOrDefault(r => r.Name.ToLower() == ingredientDto.Name.ToLower());

            //Ha az ingredient nem letezik meg az adatbazisban hozza adja
            if (ingredient == null)
            {
                ingredient = new Ingredient
                {
                    Name = ingredientDto.Name.ToLower(),
                    Recipes = new List<Recipes>()
                };
                await _repository.AddIngredient(ingredient);
            }
            recipe.Ingredient.Add(ingredient);
        }

        await _repository.AddRecipe(recipe);
    }

    public async Task UpdateRecipe(RecipeDto1 updtRecipe)
    {
        //Megkeresi a receptet amit frissiteni szeretnenk
        var recipe = await _repository.GetAllRecipes().FirstOrDefaultAsync(r => r.Id == updtRecipe.Id);

        //Nincs az adatbazisban ilyen
        if (recipe == null)
        {
            throw new KeyNotFoundException("Wrong Id for recipe!");
        }

        //Recept elemeit feltoltsuk
        recipe.Name = updtRecipe.Name.ToLower();
        recipe.Description = updtRecipe.Description.ToLower();
        recipe.Image = updtRecipe.Image;

        //Az ingredientek hozzaadasa a recepthez, es ha nincs akkor letrehozza az ujat
        if (updtRecipe.Ingredient != null && updtRecipe.Ingredient.Any())
        {
            foreach (var ingredientDto in updtRecipe.Ingredient)
            {
                var existingIngredient = await _repository.GetAllIngredients().FirstOrDefaultAsync(i => i.Name == ingredientDto.Name);

                if (existingIngredient == null)
                {
                    existingIngredient = new Ingredient
                    {
                        Name = ingredientDto.Name.ToLower(),
                        Recipes = new List<Recipes>()
                    };
                    await _repository.AddIngredient(existingIngredient);
                }
                else
                {
                    throw new InvalidOperationException("You can`t modify old ingredients!");
                }

                if (!recipe.Ingredient.Any(i => i.Name.ToLower() == existingIngredient.Name.ToLower()))
                {
                    recipe.Ingredient.Add(existingIngredient);
                }

                if (!existingIngredient.Recipes.Contains(recipe))
                {
                    existingIngredient.Recipes.Add(recipe);
                }
            }
        }

        await _repository.UpdateRecipe(recipe);
    }

    public async Task DeleteRecipe(int recipeId)
    {
        var recipe = await _repository.GetAllRecipes().FirstOrDefaultAsync(r => r.Id == recipeId);

        //Nincs mit magyarazni itt is, ugyanaz mint az ingredientnel
        if (recipe == null)
        {
            throw new KeyNotFoundException("Id doesn`t exists in the Recipes table!");
        }

        await _repository.DeleteRecipe(recipe);
    }
}