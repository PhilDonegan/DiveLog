using Microsoft.EntityFrameworkCore.Migrations;

namespace DiveLog.DAL.Migrations
{
    public partial class AddedCNS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "CNS",
                table: "DataPoint",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CNS",
                table: "DataPoint");
        }
    }
}
