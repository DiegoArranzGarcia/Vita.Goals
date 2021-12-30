using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Vita.Persistance.Sql.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Color = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "CreatedBy", "Name" },
                values: new object[,]
                {
                    { new Guid("afaf1663-9fac-40aa-a954-18b7c6b60e6d"), "skyblue", null, "Cooking" },
                    { new Guid("57813808-4fb0-4a43-9dd7-2088879f4451"), "crimson", null, "Sports" },
                    { new Guid("47c4002b-7e2b-4ea2-9b14-c0e7079b994f"), "darkorange", null, "VideoGames" },
                    { new Guid("cef48041-89ee-46ce-9288-ea179e51ea41"), "lightslategray", null, "Travel" },
                    { new Guid("6d89f7fa-1713-43fb-86db-23f3d58c8471"), "deeppink", null, "Study" },
                    { new Guid("0af68196-cf25-44f0-88f1-fda784074575"), "springgreen", null, "TV Series" },
                    { new Guid("0d3558b0-24ae-4b2b-9e96-f8eaf8a43a56"), "turquoise", null, "Movies" },
                    { new Guid("8af37209-2f7c-4cf1-bc96-075b835e1715"), "darkslateblue", null, "Books" },
                    { new Guid("6b9db6ae-ed80-4ccc-ba76-ffeaa110bc8c"), "limegreen", null, "Music" },
                    { new Guid("92d41e17-05a2-4721-88f3-d303d8dc9785"), "lightseagreen", null, "Places" },
                    { new Guid("f0dc8c7c-e943-456d-ac49-9c99c0ac9ab6"), "darkcyan", null, "Podcasts" },
                    { new Guid("39609025-e42a-483b-a8dc-b25de68dab28"), "dodgerblue", null, "Bar & Clubs" },
                    { new Guid("117aac89-8589-4a64-99bd-616bd86b9e3d"), "tomato", null, "Restaurants" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedBy",
                table: "Categories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
