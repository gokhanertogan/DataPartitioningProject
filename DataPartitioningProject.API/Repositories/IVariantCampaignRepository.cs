using DataPartitioningProject.API.Contexts;
using DataPartitioningProject.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataPartitioningProject.API.Repositories;

public interface IVariantCampaignRepository
{
    Task<List<VariantCampaign>> GetCampaignVariantsAsync(long campaignId);
    Task<List<long>> GetVariantCampaignsAsync(long variantId);
    Task<List<VariantCampaignMapping>> GetCampaignVariantMappingsAsync(long campaignId);
    Task<VariantCampaignMapping?> GetVariantCampaignMappingAsync(long variantId);
    Task AddVariantCampaignAsync(VariantCampaign campaign);
    Task AddVariantCampaignMappingAsync(VariantCampaignMapping mapping);
    Task SaveChangesAsync();
}

public class VariantCampaignRepository(AppDbContext context) : IVariantCampaignRepository
{
    private readonly AppDbContext _context = context;

    public async Task<List<VariantCampaign>> GetCampaignVariantsAsync(long campaignId)
    {
        return await _context.VariantCampaigns
            .AsNoTracking()
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

    public async Task<List<VariantCampaignMapping>> GetCampaignVariantMappingsAsync(long campaignId)
    {
        // Translates to "CampaignIds" @> '[campaignId]', which can use the GIN index.
        return await _context.VariantCampaignMappings
            .AsNoTracking()
            .Where(vcm => EF.Functions.JsonContains(vcm.CampaignIds, $"[{campaignId}]"))
            .ToListAsync();
    }

    public async Task<VariantCampaignMapping?> GetVariantCampaignMappingAsync(long variantId)
    {
        return await _context.VariantCampaignMappings
            .FirstOrDefaultAsync(vcm => vcm.VariantId == variantId);
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

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
