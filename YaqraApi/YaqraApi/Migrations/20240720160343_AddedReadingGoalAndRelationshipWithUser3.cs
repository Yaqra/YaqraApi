using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YaqraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedReadingGoalAndRelationshipWithUser3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ReadingGoals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "ReadingGoals");
        }
    }
}
