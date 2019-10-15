using Microsoft.EntityFrameworkCore.Migrations;

namespace DiveLog.DAL.Migrations
{
    public partial class AddedStatFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleRate",
                table: "LogEntries");

            migrationBuilder.AddColumn<int>(
                name: "AverageBottomDepth",
                table: "LogEntries",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BottomTime",
                table: "LogEntries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageBottomDepth",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "BottomTime",
                table: "LogEntries");

            migrationBuilder.AddColumn<short>(
                name: "SampleRate",
                table: "LogEntries",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
