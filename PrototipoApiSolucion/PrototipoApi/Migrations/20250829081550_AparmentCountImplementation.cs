using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototipoApi.Migrations
{
    /// <inheritdoc />
    public partial class AparmentCountImplementation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApartmentCount",
                table: "Buildings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApartmentCount",
                table: "Buildings");
        }
    }
}
