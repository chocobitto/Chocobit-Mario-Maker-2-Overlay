﻿// <auto-generated />
using System;
using MarioMaker2Overlay.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Chocobit.Shared.Migrations
{
    [DbContext(typeof(MarioMaker2OverlayContext))]
    partial class MarioMaker2OverlayContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

            modelBuilder.Entity("MarioMaker2Overlay.Persistence.LevelData", b =>
                {
                    b.Property<int>("LevelDataId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClearCondition")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ClearConditionMagnitude")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ClearTime")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .HasMaxLength(11)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateTimeCleared")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTimeStarted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateTimeUploaded")
                        .HasColumnType("TEXT");

                    b.Property<string>("Difficulty")
                        .HasColumnType("TEXT");

                    b.Property<bool>("FirstClear")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GameStyle")
                        .HasColumnType("TEXT");

                    b.Property<int>("PlayerDeaths")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tags")
                        .HasColumnType("TEXT");

                    b.Property<string>("Theme")
                        .HasColumnType("TEXT");

                    b.Property<long>("TimeElapsed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalGlobalAttempts")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalGlobalClears")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("WorldRecord")
                        .HasColumnType("INTEGER");

                    b.HasKey("LevelDataId");

                    b.HasIndex("PlayerId", "Code")
                        .IsUnique();

                    b.ToTable("LevelData");
                });

            modelBuilder.Entity("MarioMaker2Overlay.Persistence.Player", b =>
                {
                    b.Property<int>("PlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlayerName")
                        .HasMaxLength(11)
                        .HasColumnType("TEXT");

                    b.HasKey("PlayerId");

                    b.ToTable("Player");
                });
#pragma warning restore 612, 618
        }
    }
}
