using MediaWebApp.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaWebApp.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261012010000_AddNamedCollections")]
public partial class AddNamedCollections : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Keep the previous one-list-per-user table and its data for rollback.
        migrationBuilder.RenameTable(name: "MediaLists", newName: "LegacyMediaLists");

        migrationBuilder.CreateTable(
            name: "MediaCollections",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                IsPublic = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MediaCollections", x => x.Id);
                table.ForeignKey("FK_MediaCollections_AspNetUsers_OwnerId", x => x.OwnerId,
                    "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
            });
        migrationBuilder.CreateIndex("IX_MediaCollections_OwnerId", "MediaCollections", "OwnerId");

        migrationBuilder.CreateTable(
            name: "UserProfiles",
            columns: table => new
            {
                UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserProfiles", x => x.UserId);
                table.ForeignKey("FK_UserProfiles_AspNetUsers_UserId", x => x.UserId,
                    "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.Sql("""
            INSERT INTO [UserProfiles] ([UserId], [DisplayName])
            SELECT [Id], CONCAT('Member-', LEFT(REPLACE([Id], '-', ''), 8)) FROM [AspNetUsers];

            INSERT INTO [MediaCollections] ([OwnerId], [Name], [IsPublic])
            SELECT u.[Id],
                   COALESCE(NULLIF(LTRIM(RTRIM(old.[DisplayName])), ''), 'Favorites'),
                   COALESCE(old.[IsPublic], 0)
            FROM [AspNetUsers] u
            LEFT JOIN [LegacyMediaLists] old ON old.[UserId] = u.[Id]
            WHERE old.[UserId] IS NOT NULL
               OR EXISTS (SELECT 1 FROM [MediaEntries] e WHERE e.[OwnerId] = u.[Id]);
            """);

        migrationBuilder.AddColumn<int>(name: "ListId", table: "MediaEntries", type: "int", nullable: true);
        migrationBuilder.Sql("""
            UPDATE e SET [ListId] = l.[Id]
            FROM [MediaEntries] e JOIN [MediaCollections] l ON l.[OwnerId] = e.[OwnerId];
            """);
        migrationBuilder.AlterColumn<int>(name: "ListId", table: "MediaEntries", type: "int",
            nullable: false, oldClrType: typeof(int), oldType: "int", oldNullable: true);
        migrationBuilder.CreateIndex("IX_MediaEntries_ListId", "MediaEntries", "ListId");
        migrationBuilder.AddForeignKey("FK_MediaEntries_MediaCollections_ListId", "MediaEntries", "ListId",
            "MediaCollections", principalColumn: "Id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("FK_MediaEntries_MediaCollections_ListId", "MediaEntries");
        migrationBuilder.DropIndex("IX_MediaEntries_ListId", "MediaEntries");
        migrationBuilder.DropColumn("ListId", "MediaEntries");
        migrationBuilder.DropTable("MediaCollections");
        migrationBuilder.DropTable("UserProfiles");
        migrationBuilder.RenameTable(name: "LegacyMediaLists", newName: "MediaLists");
    }
}
