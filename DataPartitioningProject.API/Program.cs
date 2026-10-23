using DataPartitioningProject.API.Contexts;
using DataPartitioningProject.API.Repositories;
using DataPartitioningProject.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
// SQL commands go through the standard logging pipeline (category "Microsoft.EntityFrameworkCore.Database.Command").
// They are off by default; turn them on in configuration, e.g. Logging__LogLevel__Microsoft.EntityFrameworkCore.Database.Command=Information.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection")));

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
    // Upper bound is exclusive: seeded campaign ids are 1..1000.
    var campaignId = Random.Shared.Next(1, 1001);

    var variants = await dbContext.VariantCampaigns
        .Where(v => v.CampaignId == campaignId)
        .ToListAsync();

    return Results.Ok(variants);
});

// Partitioned table: WHERE campaignid = @id touches a single partition.
app.MapGet("/campaigns/{campaignId:long}/variants", async (long campaignId, IVariantCampaignService service) =>
    Results.Ok(await service.GetCampaignVariants(campaignId)));

// jsonb mapping table: containment query served by the GIN index.
app.MapGet("/campaigns/{campaignId:long}/variant-mappings", async (long campaignId, IVariantCampaignService service) =>
    Results.Ok(await service.GetCampaignVariantMappings(campaignId)));

app.MapGet("/variants/{variantId:long}/campaigns", async (long variantId, IVariantCampaignService service) =>
    Results.Ok(await service.GetVariantCampaigns(variantId)));

app.MapPost("/variants/{variantId:long}/campaigns", async (long variantId, AddVariantToCampaignRequest request, IVariantCampaignService service) =>
{
    await service.AddVariantToCampaign(variantId, request.CampaignId, request.SalePrice);
    return Results.NoContent();
});

app.Run();

record AddVariantToCampaignRequest(long CampaignId, decimal SalePrice);
