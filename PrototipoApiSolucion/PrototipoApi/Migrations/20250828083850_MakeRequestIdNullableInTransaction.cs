using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototipoApi.Migrations
{
    /// <inheritdoc />
    public partial class MakeRequestIdNullableInTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Requests_RequestId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "RequestId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Requests_RequestId",
                table: "Transactions",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Requests_RequestId",
                table: "Transactions");

            migrationBuilder.AlterColumn<int>(
                name: "RequestId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Requests_RequestId",
                table: "Transactions",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
