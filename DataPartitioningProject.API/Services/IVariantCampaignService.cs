using DataPartitioningProject.API.Entities;
using DataPartitioningProject.API.Repositories;

namespace DataPartitioningProject.API.Services;

public interface IVariantCampaignService
{
    Task<List<VariantCampaign>> GetCampaignVariants(long campaignId);
    Task<List<long>> GetVariantCampaigns(long variantId);
    Task<List<VariantCampaignMapping>> GetCampaignVariantMappings(long campaignId);
    Task AddVariantToCampaign(long variantId, long campaignId, decimal salePrice);
}

public class VariantCampaignService(IVariantCampaignRepository repository) : IVariantCampaignService
{
    private readonly IVariantCampaignRepository _repository = repository;

    public async Task<List<VariantCampaign>> GetCampaignVariants(long campaignId)
    {
        return await _repository.GetCampaignVariantsAsync(campaignId);
    }

    public async Task<List<long>> GetVariantCampaigns(long variantId)
    {
        return await _repository.GetVariantCampaignsAsync(variantId);
    }

    public async Task<List<VariantCampaignMapping>> GetCampaignVariantMappings(long campaignId)
    {
        return await _repository.GetCampaignVariantMappingsAsync(campaignId);
    }

    public async Task AddVariantToCampaign(long variantId, long campaignId, decimal salePrice)
    {
        // One mapping row per variant: update it instead of inserting a duplicate, and make the call idempotent.
        var mapping = await _repository.GetVariantCampaignMappingAsync(variantId);
        if (mapping is not null && mapping.CampaignIds.Contains(campaignId))
        {
            return;
        }

        await _repository.AddVariantCampaignAsync(new VariantCampaign
        {
            VariantId = variantId,
            CampaignId = campaignId,
            VariantSalePrice = salePrice
        });

        if (mapping is null)
        {
            await _repository.AddVariantCampaignMappingAsync(new VariantCampaignMapping
            {
                VariantId = variantId,
                CampaignIds = [campaignId],
                VariantSalePrice = salePrice
            });
            return;
        }

        mapping.CampaignIds.Add(campaignId);
        await _repository.SaveChangesAsync();
    }
}
