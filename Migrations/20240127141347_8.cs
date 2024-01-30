using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatingApp.Migrations
{
    /// <inheritdoc />
    public partial class _8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountChat_Accounts_AccountId",
                table: "AccountChat");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountChat_Chats_ChatId",
                table: "AccountChat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountChat",
                table: "AccountChat");

            migrationBuilder.RenameTable(
                name: "AccountChat",
                newName: "AccountChats");

            migrationBuilder.RenameIndex(
                name: "IX_AccountChat_ChatId",
                table: "AccountChats",
                newName: "IX_AccountChats_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountChats",
                table: "AccountChats",
                columns: new[] { "AccountId", "ChatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AccountChats_Accounts_AccountId",
                table: "AccountChats",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountChats_Chats_ChatId",
                table: "AccountChats",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountChats_Accounts_AccountId",
                table: "AccountChats");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountChats_Chats_ChatId",
                table: "AccountChats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountChats",
                table: "AccountChats");

            migrationBuilder.RenameTable(
                name: "AccountChats",
                newName: "AccountChat");

            migrationBuilder.RenameIndex(
                name: "IX_AccountChats_ChatId",
                table: "AccountChat",
                newName: "IX_AccountChat_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountChat",
                table: "AccountChat",
                columns: new[] { "AccountId", "ChatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AccountChat_Accounts_AccountId",
                table: "AccountChat",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountChat_Chats_ChatId",
                table: "AccountChat",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
