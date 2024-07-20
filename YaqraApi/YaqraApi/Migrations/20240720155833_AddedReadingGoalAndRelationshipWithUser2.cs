using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YaqraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedReadingGoalAndRelationshipWithUser2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReadingGoal_AspNetUsers_UserId",
                table: "ReadingGoal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReadingGoal",
                table: "ReadingGoal");

            migrationBuilder.RenameTable(
                name: "ReadingGoal",
                newName: "ReadingGoals");

            migrationBuilder.RenameIndex(
                name: "IX_ReadingGoal_UserId",
                table: "ReadingGoals",
                newName: "IX_ReadingGoals_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReadingGoals",
                table: "ReadingGoals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReadingGoals_AspNetUsers_UserId",
                table: "ReadingGoals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReadingGoals_AspNetUsers_UserId",
                table: "ReadingGoals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReadingGoals",
                table: "ReadingGoals");

            migrationBuilder.RenameTable(
                name: "ReadingGoals",
                newName: "ReadingGoal");

            migrationBuilder.RenameIndex(
                name: "IX_ReadingGoals_UserId",
                table: "ReadingGoal",
                newName: "IX_ReadingGoal_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReadingGoal",
                table: "ReadingGoal",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReadingGoal_AspNetUsers_UserId",
                table: "ReadingGoal",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
