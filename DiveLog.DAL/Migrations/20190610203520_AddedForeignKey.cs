using Microsoft.EntityFrameworkCore.Migrations;

namespace DiveLog.DAL.Migrations
{
    public partial class AddedForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataPoint_LogEntries_LogEntryId",
                table: "DataPoint");

            migrationBuilder.AlterColumn<long>(
                name: "LogEntryId",
                table: "DataPoint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Depth",
                table: "DataPoint",
                type: "decimal(4, 1)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddForeignKey(
                name: "FK_DataPoint_LogEntries_LogEntryId",
                table: "DataPoint",
                column: "LogEntryId",
                principalTable: "LogEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataPoint_LogEntries_LogEntryId",
                table: "DataPoint");

            migrationBuilder.AlterColumn<long>(
                name: "LogEntryId",
                table: "DataPoint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<decimal>(
                name: "Depth",
                table: "DataPoint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4, 1)");

            migrationBuilder.AddForeignKey(
                name: "FK_DataPoint_LogEntries_LogEntryId",
                table: "DataPoint",
                column: "LogEntryId",
                principalTable: "LogEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
