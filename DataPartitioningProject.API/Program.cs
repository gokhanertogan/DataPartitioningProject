using DataPartitioningProject.API.Contexts;
using DataPartitioningProject.API.Repositories;
using DataPartitioningProject.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")).LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

builder.Services.AddScoped<IVariantCampaignRepository, VariantCampaignRepository>();
builder.Services.AddScoped<IVariantCampaignService, VariantCampaignService>();
builder.Services.AddScoped<VariantCampaignDataSeeder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<VariantCampaignDataSeeder>();
    await seeder.SeedAsync();
}
app.UseHttpsRedirection();

app.MapGet("/variant-campaigns", async (AppDbContext dbContext) =>
{
    var random = new Random();
    var campaignId = random.Next(1, 1000);

    var variants = await dbContext.VariantCampaigns
        .Where(v => v.CampaignId == campaignId)
        .ToListAsync();

    return Results.Ok(variants);
});

app.Run();



