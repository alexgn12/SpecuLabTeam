using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrototipoApi.Migrations
{
    /// <inheritdoc />
    public partial class inicial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatusHistory_Requests_RequestId",
                table: "RequestStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatusHistory_Statuses_NewStatusId",
                table: "RequestStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatusHistory_Statuses_OldStatusId",
                table: "RequestStatusHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestStatusHistory",
                table: "RequestStatusHistory");

            migrationBuilder.RenameTable(
                name: "RequestStatusHistory",
                newName: "RequestStatusHistories");

            migrationBuilder.RenameIndex(
                name: "IX_RequestStatusHistory_RequestId",
                table: "RequestStatusHistories",
                newName: "IX_RequestStatusHistories_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestStatusHistory_OldStatusId",
                table: "RequestStatusHistories",
                newName: "IX_RequestStatusHistories_OldStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestStatusHistory_NewStatusId",
                table: "RequestStatusHistories",
                newName: "IX_RequestStatusHistories_NewStatusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestStatusHistories",
                table: "RequestStatusHistories",
                column: "RequestStatusHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatusHistories_Requests_RequestId",
                table: "RequestStatusHistories",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatusHistories_Statuses_NewStatusId",
                table: "RequestStatusHistories",
                column: "NewStatusId",
                principalTable: "Statuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatusHistories_Statuses_OldStatusId",
                table: "RequestStatusHistories",
                column: "OldStatusId",
                principalTable: "Statuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatusHistories_Requests_RequestId",
                table: "RequestStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatusHistories_Statuses_NewStatusId",
                table: "RequestStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatusHistories_Statuses_OldStatusId",
                table: "RequestStatusHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestStatusHistories",
                table: "RequestStatusHistories");

            migrationBuilder.RenameTable(
                name: "RequestStatusHistories",
                newName: "RequestStatusHistory");

            migrationBuilder.RenameIndex(
                name: "IX_RequestStatusHistories_RequestId",
                table: "RequestStatusHistory",
                newName: "IX_RequestStatusHistory_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestStatusHistories_OldStatusId",
                table: "RequestStatusHistory",
                newName: "IX_RequestStatusHistory_OldStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestStatusHistories_NewStatusId",
                table: "RequestStatusHistory",
                newName: "IX_RequestStatusHistory_NewStatusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestStatusHistory",
                table: "RequestStatusHistory",
                column: "RequestStatusHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatusHistory_Requests_RequestId",
                table: "RequestStatusHistory",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatusHistory_Statuses_NewStatusId",
                table: "RequestStatusHistory",
                column: "NewStatusId",
                principalTable: "Statuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatusHistory_Statuses_OldStatusId",
                table: "RequestStatusHistory",
                column: "OldStatusId",
                principalTable: "Statuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
