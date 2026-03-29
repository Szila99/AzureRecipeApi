using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
	builder.Configuration.GetConnectionString("DefaultConnection") ??
	Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
	throw new InvalidOperationException(
		"Database connection string is missing. Configure ConnectionStrings__DefaultConnection (App Settings) " +
		"or DefaultConnection in Azure App Service Connection strings.");
}

builder.Services.AddDbContext<Recipe.RecipeContext>(options =>
	options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<Recipe.RecipeContext>();
	var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

	try
	{
		db.Database.EnsureCreated();
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "Database initialization failed during startup.");
		throw;
	}
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => Results.Ok("A Backend fut!"));

app.Run();