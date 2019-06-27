using Microsoft.EntityFrameworkCore.Migrations;

namespace DiveLog.DAL.Migrations
{
    public partial class HashCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HashCode",
                table: "LogEntries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashCode",
                table: "LogEntries");
        }
    }
}
