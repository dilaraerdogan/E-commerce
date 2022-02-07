using Microsoft.EntityFrameworkCore.Migrations;

namespace BookShopApp.data.Migrations
{
    public partial class UpdateInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StokAdedi",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StokAdedi",
                table: "Products");
        }
    }
}
