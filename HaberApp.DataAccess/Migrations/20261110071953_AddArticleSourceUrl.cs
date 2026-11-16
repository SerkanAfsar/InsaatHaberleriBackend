using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HaberApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleSourceUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "Articles");
        }
    }
}
