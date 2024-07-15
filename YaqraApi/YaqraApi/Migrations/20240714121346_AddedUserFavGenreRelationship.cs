using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YaqraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserFavGenreRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFavouriteGenres",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavouriteGenres", x => new { x.UserId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_UserFavouriteGenres_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavouriteGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavouriteGenres_GenreId",
                table: "UserFavouriteGenres",
                column: "GenreId");
            migrationBuilder.CreateIndex(
                name: "IX_UserFavouriteGenres_UserId",
                table: "UserFavouriteGenres",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavouriteGenres");
        }
    }
}
