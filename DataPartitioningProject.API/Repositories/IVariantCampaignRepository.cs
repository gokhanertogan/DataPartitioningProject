using DataPartitioningProject.API.Contexts;
using DataPartitioningProject.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataPartitioningProject.API.Repositories;

public interface IVariantCampaignRepository
{
    Task<List<VariantCampaign>> GetCampaignVariantsAsync(long campaignId);
    Task<List<long>> GetVariantCampaignsAsync(long variantId);
    Task AddVariantCampaignAsync(VariantCampaign campaign);
    Task AddVariantCampaignMappingAsync(VariantCampaignMapping mapping);
}

public class VariantCampaignRepository(AppDbContext context) : IVariantCampaignRepository
{
    private readonly AppDbContext _context = context;

    public async Task<List<VariantCampaign>> GetCampaignVariantsAsync(long campaignId)
    {
        return await _context.VariantCampaigns
            .Where(vc => vc.CampaignId == campaignId)
            .ToListAsync();
    }

    public async Task<List<long>> GetVariantCampaignsAsync(long variantId)
    {
        var result = await _context.VariantCampaignMappings
            .Where(vcm => vcm.VariantId == variantId)
            .Select(vcm => vcm.CampaignIds)
            .FirstOrDefaultAsync();

        return result ?? [];
    }

    public async Task AddVariantCampaignAsync(VariantCampaign campaign)
    {
        _context.VariantCampaigns.Add(campaign);
        await _context.SaveChangesAsync();
    }

    public async Task AddVariantCampaignMappingAsync(VariantCampaignMapping mapping)
    {
        _context.VariantCampaignMappings.Add(mapping);
        await _context.SaveChangesAsync();
    }
}
