using Microsoft.EntityFrameworkCore.Migrations;
using YaqraApi.Helpers;

#nullable disable

namespace YaqraApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] {"Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    {Guid.NewGuid().ToString(), Roles.Admin, Roles.Admin.ToUpper(), Guid.NewGuid().ToString() },
                    {Guid.NewGuid().ToString(), Roles.User, Roles.User.ToUpper(), Guid.NewGuid().ToString() }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [AspNetRoles]");
        }
    }
}
