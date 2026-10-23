using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataPartitioningProject.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VariantCampaigns_campaignid",
                table: "VariantCampaigns",
                column: "campaignid");

            migrationBuilder.CreateIndex(
                name: "IX_VariantCampaignMappings_CampaignIds",
                table: "VariantCampaignMappings",
                column: "CampaignIds")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_VariantCampaignMappings_VariantId",
                table: "VariantCampaignMappings",
                column: "VariantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VariantCampaigns_campaignid",
                table: "VariantCampaigns");

            migrationBuilder.DropIndex(
                name: "IX_VariantCampaignMappings_CampaignIds",
                table: "VariantCampaignMappings");

            migrationBuilder.DropIndex(
                name: "IX_VariantCampaignMappings_VariantId",
                table: "VariantCampaignMappings");
        }
    }
}
