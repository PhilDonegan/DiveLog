using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiveLog.DAL.Migrations
{
    public partial class AddedMoreFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "DiveLength",
                table: "LogEntries",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<decimal>(
                name: "FractionHe",
                table: "LogEntries",
                type: "decimal(4, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FractionO2",
                table: "LogEntries",
                type: "decimal(4, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDepth",
                table: "LogEntries",
                type: "decimal(4, 1)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Outcome",
                table: "LogEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AveragePPO2",
                table: "DataPoint",
                type: "decimal(3, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "WaterTemp",
                table: "DataPoint",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiveLength",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "FractionHe",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "FractionO2",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "MaxDepth",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "AveragePPO2",
                table: "DataPoint");

            migrationBuilder.DropColumn(
                name: "WaterTemp",
                table: "DataPoint");
        }
    }
}
