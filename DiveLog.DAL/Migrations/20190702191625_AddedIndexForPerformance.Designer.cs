﻿// <auto-generated />
using System;
using DiveLog.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiveLog.DAL.Migrations
{
    [DbContext(typeof(DiveLogContext))]
    [Migration("20190702191625_AddedIndexForPerformance")]
    partial class AddedIndexForPerformance
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DiveLog.DAL.Models.DataPoint", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("AveragePPO2")
                        .HasColumnType("decimal(3, 2)");

                    b.Property<decimal>("Depth")
                        .HasColumnType("decimal(4, 1)");

                    b.Property<long>("LogEntryId");

                    b.Property<int>("Time");

                    b.Property<short>("WaterTemp");

                    b.HasKey("Id");

                    b.HasIndex("LogEntryId");

                    b.ToTable("DataPoint");
                });

            modelBuilder.Entity("DiveLog.DAL.Models.LogEntry", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DiveDate");

                    b.Property<TimeSpan>("DiveLength");

                    b.Property<int>("DiveType");

                    b.Property<string>("ExternalId");

                    b.Property<decimal>("FractionHe")
                        .HasColumnType("decimal(4, 2)");

                    b.Property<decimal>("FractionO2")
                        .HasColumnType("decimal(4, 2)");

                    b.Property<string>("HashCode");

                    b.Property<decimal>("MaxDepth")
                        .HasColumnType("decimal(4, 1)");

                    b.Property<int>("Outcome");

                    b.Property<short>("SampleRate");

                    b.HasKey("Id");

                    b.HasIndex("HashCode")
                        .IsUnique()
                        .HasFilter("[HashCode] IS NOT NULL");

                    b.ToTable("LogEntries");
                });

            modelBuilder.Entity("DiveLog.DAL.Models.DataPoint", b =>
                {
                    b.HasOne("DiveLog.DAL.Models.LogEntry")
                        .WithMany("DataPoints")
                        .HasForeignKey("LogEntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
