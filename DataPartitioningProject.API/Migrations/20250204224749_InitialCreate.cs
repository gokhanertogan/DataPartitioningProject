using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataPartitioningProject.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE ""VariantCampaigns"" (
                    VariantId BIGINT,
                    CampaignId BIGINT,
                    VariantSalePrice DECIMAL(10,2),
                    PRIMARY KEY (VariantId, CampaignId)
                ) PARTITION BY RANGE (CampaignId);
            ");

            migrationBuilder.Sql(@"CREATE TABLE ""VariantCampaigns_1_200"" PARTITION OF ""VariantCampaigns"" FOR VALUES FROM (1) TO (201)");
            migrationBuilder.Sql(@"CREATE TABLE ""VariantCampaigns_1_400"" PARTITION OF ""VariantCampaigns"" FOR VALUES FROM (201) TO (401)");
            migrationBuilder.Sql(@"CREATE TABLE ""VariantCampaigns_1_600"" PARTITION OF ""VariantCampaigns"" FOR VALUES FROM (401) TO (601)");
            migrationBuilder.Sql(@"CREATE TABLE ""VariantCampaigns_1_800"" PARTITION OF ""VariantCampaigns"" FOR VALUES FROM (601) TO (801)");
            migrationBuilder.Sql(@"CREATE TABLE ""VariantCampaigns_1_1000"" PARTITION OF ""VariantCampaigns"" FOR VALUES FROM (801) TO (1001)");
            migrationBuilder.Sql(@"CREATE TABLE ""VariantCampaigns_1_1200"" PARTITION OF ""VariantCampaigns"" FOR VALUES FROM (1001) TO (1201)");

            // migrationBuilder.Sql(@"
            //     CREATE TABLE ""VariantCampaigns_2"" PARTITION OF ""VariantCampaigns""
            //         FOR VALUES IN (2);
            // ");

            // migrationBuilder.Sql(@"
            // CREATE TABLE ""VariantCampaigns_3"" PARTITION OF ""VariantCampaigns""
            //     FOR VALUES IN (3);
            //");

            migrationBuilder.CreateTable(
                name: "VariantCampaignMappings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VariantId = table.Column<long>(type: "bigint", nullable: false),
                    CampaignIds = table.Column<string>(type: "jsonb", nullable: false),
                    VariantSalePrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantCampaignMappings", x => x.Id);
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""VariantCampaigns_1_200"";");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""VariantCampaigns_1_400"";");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""VariantCampaigns_1_600"";");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""VariantCampaigns_1_800"";");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""VariantCampaigns_1_1000"";");
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""VariantCampaigns_1_1200"";");

            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""VariantCampaigns"";");

            migrationBuilder.DropTable(
                name: "VariantCampaignMappings");
        }
    }
}
