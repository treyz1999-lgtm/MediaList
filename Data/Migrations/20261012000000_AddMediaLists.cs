using MediaWebApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaWebApp.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261012000000_AddMediaLists")]
public partial class AddMediaLists : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MediaLists",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                IsPublic = table.Column<bool>(type: "bit", nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MediaLists", x => x.UserId);
                table.ForeignKey("FK_MediaLists_AspNetUsers_UserId", x => x.UserId,
                    "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "MediaEntries",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                MediaType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                Genre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                Rating = table.Column<int>(type: "int", nullable: false),
                CoverImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MediaEntries", x => x.Id);
                table.ForeignKey("FK_MediaEntries_AspNetUsers_OwnerId", x => x.OwnerId,
                    "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_MediaEntries_OwnerId", "MediaEntries", "OwnerId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("MediaEntries");
        migrationBuilder.DropTable("MediaLists");
    }
}
