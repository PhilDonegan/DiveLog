using Microsoft.EntityFrameworkCore.Migrations;

namespace DiveLog.DAL.Migrations
{
    public partial class AddedIndexForPerformance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HashCode",
                table: "LogEntries",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_HashCode",
                table: "LogEntries",
                column: "HashCode",
                unique: true,
                filter: "[HashCode] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LogEntries_HashCode",
                table: "LogEntries");

            migrationBuilder.AlterColumn<string>(
                name: "HashCode",
                table: "LogEntries",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
