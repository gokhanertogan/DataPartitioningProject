using Microsoft.EntityFrameworkCore.Migrations;

namespace DataPartitioningProject.API.Indexes;

public class AddGINIndexToCampaignIds : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_VariantCampaignMappings_CampaignIds",
            table: "VariantCampaignMappings",
            column: "CampaignIds")
            .Annotation("Npgsql:IndexMethod", "GIN");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_VariantCampaignMappings_CampaignIds",
            table: "VariantCampaignMappings");
    }
}