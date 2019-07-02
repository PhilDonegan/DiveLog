using Microsoft.EntityFrameworkCore.Migrations;

namespace DiveLog.DAL.Migrations
{
    public partial class AddedExternalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "LogEntries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "LogEntries");
        }
    }
}
