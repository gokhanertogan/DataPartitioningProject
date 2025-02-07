namespace DataPartitioningProject.API.Entities;

public class VariantCampaignMapping
{
    public long Id { get; set; }
    public long VariantId { get; set; }
    public List<long> CampaignIds { get; set; } = [];
    public decimal VariantSalePrice { get; set; }

}