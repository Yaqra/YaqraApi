using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YaqraApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedBooksPlaylistsBooksDiscussionArticleNewsRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscussionArticleNewsBooks",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "int", nullable: false),
                    DiscussionArticleNewsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionArticleNewsBooks", x => new { x.BookId, x.DiscussionArticleNewsId });
                    table.ForeignKey(
                        name: "FK_DiscussionArticleNewsBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscussionArticleNewsBooks_DiscussionArticleNews_DiscussionArticleNewsId",
                        column: x => x.DiscussionArticleNewsId,
                        principalTable: "DiscussionArticleNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistBooks",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "int", nullable: false),
                    PlaylistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistBooks", x => new { x.BookId, x.PlaylistId });
                    table.ForeignKey(
                        name: "FK_PlaylistBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistBooks_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionArticleNewsBooks_DiscussionArticleNewsId",
                table: "DiscussionArticleNewsBooks",
                column: "DiscussionArticleNewsId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistBooks_PlaylistId",
                table: "PlaylistBooks",
                column: "PlaylistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscussionArticleNewsBooks");

            migrationBuilder.DropTable(
                name: "PlaylistBooks");
        }
    }
}
