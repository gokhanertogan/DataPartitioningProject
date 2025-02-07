using Bogus;
using DataPartitioningProject.API.Contexts;
using DataPartitioningProject.API.Entities;
using Microsoft.EntityFrameworkCore;

public class VariantCampaignDataSeeder(AppDbContext context)
{
    private readonly AppDbContext _context = context;
    private readonly Faker _faker = new();

    public async Task SeedAsync()
    {
        if (await _context.VariantCampaignMappings.AsNoTracking().AnyAsync())
        {
            Console.WriteLine("Data already exists, skipping seeding...");
            return;
        }

        var campaignIds = Enumerable.Range(1, 1000).Select(x => (long)x).ToList();
        var variantCampaignMappings = new List<VariantCampaignMapping>();
        var variantCampaigns = new List<VariantCampaign>();

        for (long variantId = 1; variantId <= 2_000_000; variantId++)
        {
            var salePrice = _faker.Random.Decimal(10, 500);
            var selectedCampaigns = _faker.Random.ListItems(campaignIds, _faker.Random.Int(1, 15)).ToList();

            variantCampaignMappings.Add(new VariantCampaignMapping
            {
                VariantId = variantId,
                CampaignIds = selectedCampaigns,
                VariantSalePrice = salePrice
            });

            foreach (var campaignId in selectedCampaigns)
            {
                variantCampaigns.Add(new VariantCampaign
                {
                    VariantId = variantId,
                    CampaignId = campaignId,
                    VariantSalePrice = salePrice
                });
            }

            if (variantId % 50_000 == 0)
            {
                await _context.VariantCampaignMappings.AddRangeAsync(variantCampaignMappings);
                await _context.VariantCampaigns.AddRangeAsync(variantCampaigns);
                await _context.SaveChangesAsync();

                variantCampaignMappings.Clear();
                variantCampaigns.Clear();

                Console.WriteLine($"{variantId} records added...");
                Thread.Sleep(5000);
            }
        }

        if (variantCampaignMappings.Count > 0)
        {
            await _context.VariantCampaignMappings.AddRangeAsync(variantCampaignMappings);
            await _context.VariantCampaigns.AddRangeAsync(variantCampaigns);
            await _context.SaveChangesAsync();
            Console.WriteLine("Remaining records added...");
        }
    }
}
