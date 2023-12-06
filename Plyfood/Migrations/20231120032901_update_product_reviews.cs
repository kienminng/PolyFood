using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plyfood.Migrations
{
    /// <inheritdoc />
    public partial class update_product_reviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ponit_Evaluation",
                table: "ProductReviews",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ponit_Evaluation",
                table: "ProductReviews");
        }
    }
}
