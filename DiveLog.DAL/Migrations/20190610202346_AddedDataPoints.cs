using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiveLog.DAL.Migrations
{
    public partial class AddedDataPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiveType",
                table: "LogEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "SampleRate",
                table: "LogEntries",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "DataPoint",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Time = table.Column<int>(nullable: false),
                    Depth = table.Column<decimal>(nullable: false),
                    LogEntryId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataPoint_LogEntries_LogEntryId",
                        column: x => x.LogEntryId,
                        principalTable: "LogEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataPoint_LogEntryId",
                table: "DataPoint",
                column: "LogEntryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataPoint");

            migrationBuilder.DropColumn(
                name: "DiveType",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "SampleRate",
                table: "LogEntries");
        }
    }
}
