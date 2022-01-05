using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    public partial class AddPostModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ThumbnailImagePath = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Excerpt = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    PublishDate = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    Author = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "PostId", "Author", "CategoryId", "Content", "Excerpt", "IsPublished", "PublishDate", "ThumbnailImagePath", "Title" },
                values: new object[] { 1, "admin", 1, "", "This is an excerpt for post 1", true, "05/01/22 12:36:46 AM", "/uploads/placeholder.jpg", "Post 1" });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "PostId", "Author", "CategoryId", "Content", "Excerpt", "IsPublished", "PublishDate", "ThumbnailImagePath", "Title" },
                values: new object[] { 2, "admin", 1, "", "This is an excerpt for post 2", true, "05/01/22 12:36:46 AM", "/uploads/placeholder.jpg", "Post 2" });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "PostId", "Author", "CategoryId", "Content", "Excerpt", "IsPublished", "PublishDate", "ThumbnailImagePath", "Title" },
                values: new object[] { 3, "admin", 2, "", "This is an excerpt for post 3", true, "05/01/22 12:36:46 AM", "/uploads/placeholder.jpg", "Post 3" });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "PostId", "Author", "CategoryId", "Content", "Excerpt", "IsPublished", "PublishDate", "ThumbnailImagePath", "Title" },
                values: new object[] { 4, "admin", 2, "", "This is an excerpt for post 4", true, "05/01/22 12:36:46 AM", "/uploads/placeholder.jpg", "Post 4" });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "PostId", "Author", "CategoryId", "Content", "Excerpt", "IsPublished", "PublishDate", "ThumbnailImagePath", "Title" },
                values: new object[] { 5, "admin", 3, "", "This is an excerpt for post 5", true, "05/01/22 12:36:46 AM", "/uploads/placeholder.jpg", "Post 5" });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "PostId", "Author", "CategoryId", "Content", "Excerpt", "IsPublished", "PublishDate", "ThumbnailImagePath", "Title" },
                values: new object[] { 6, "admin", 3, "", "This is an excerpt for post 6", true, "05/01/22 12:36:46 AM", "/uploads/placeholder.jpg", "Post 6" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
