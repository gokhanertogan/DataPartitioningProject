using DataPartitioningProject.API.Entities;
using DataPartitioningProject.API.Repositories;

namespace DataPartitioningProject.API.Services;

public interface IVariantCampaignService
{
    Task<List<VariantCampaign>> GetCampaignVariants(long campaignId);
    Task<List<long>> GetVariantCampaigns(long variantId);
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

    public async Task AddVariantToCampaign(long variantId, long campaignId, decimal salePrice)
    {
        await _repository.AddVariantCampaignAsync(new VariantCampaign
        {
            VariantId = variantId,
            CampaignId = campaignId,
            VariantSalePrice = salePrice
        });

        var existingCampaigns = await _repository.GetVariantCampaignsAsync(variantId);
        existingCampaigns.Add(campaignId);

        await _repository.AddVariantCampaignMappingAsync(new VariantCampaignMapping
        {
            VariantId = variantId,
            CampaignIds = existingCampaigns
        });
    }
}
