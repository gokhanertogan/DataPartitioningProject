using System.Text.Json;
using DataPartitioningProject.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DataPartitioningProject.API.Contexts;

public class AppDbContext : DbContext
{
    public DbSet<VariantCampaign> VariantCampaigns { get; set; }
    public DbSet<VariantCampaignMapping> VariantCampaignMappings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VariantCampaign>()
            .HasKey(vc => new { vc.VariantId, vc.CampaignId });

        modelBuilder.Entity<VariantCampaign>()
        .Property(v => v.VariantId)
        .HasColumnName("variantid");

        modelBuilder.Entity<VariantCampaign>()
            .Property(v => v.CampaignId)
            .HasColumnName("campaignid");

        // The primary key is (variantid, campaignid), so lookups by campaign alone need their own index.
        modelBuilder.Entity<VariantCampaign>()
            .HasIndex(v => v.CampaignId);

        modelBuilder.Entity<VariantCampaign>()
            .Property(v => v.VariantSalePrice)
            .HasColumnName("variantsaleprice");

        modelBuilder.Entity<VariantCampaignMapping>()
         .HasKey(v => v.Id);

        modelBuilder.Entity<VariantCampaignMapping>()
            .HasIndex(v => v.VariantId)
            .IsUnique();

        // GIN index so jsonb containment queries (CampaignIds @> '[42]') do not scan the whole table.
        modelBuilder.Entity<VariantCampaignMapping>()
            .HasIndex(v => v.CampaignIds)
            .HasMethod("gin");

        modelBuilder.Entity<VariantCampaignMapping>()
            .Property(v => v.CampaignIds)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<long>>(v, JsonSerializerOptions.Default) ?? new List<long>()
            )
            .Metadata.SetValueComparer(
                new ValueComparer<List<long>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (agg, item) => agg ^ item.GetHashCode()),
                    c => c.ToList())
            );

        base.OnModelCreating(modelBuilder);
    }
}