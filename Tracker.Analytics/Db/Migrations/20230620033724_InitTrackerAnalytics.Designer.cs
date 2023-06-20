﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tracker.Analytics.Db;

#nullable disable

namespace Tracker.Analytics.Db.Migrations
{
    [DbContext(typeof(AnalyticsDbContext))]
    [Migration("20230620033724_InitTrackerAnalytics")]
    partial class InitTrackerAnalytics
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tracker.Analytics.Db.Models.Instruction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatorId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("creator_id");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deadline");

                    b.Property<DateTime?>("ExecDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("exec_date");

                    b.Property<string>("ExecutorId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("executor_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.Property<int>("StatusId")
                        .HasColumnType("integer")
                        .HasColumnName("status_id");

                    b.HasKey("Id")
                        .HasName("pk_instructions");

                    b.HasIndex("CreatorId")
                        .HasDatabaseName("ix_instructions_creator_id");

                    b.HasIndex("ExecutorId")
                        .HasDatabaseName("ix_instructions_executor_id");

                    b.ToTable("instructions", (string)null);
                });

            modelBuilder.Entity("Tracker.Analytics.Db.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Tracker.Analytics.Db.Models.Instruction", b =>
                {
                    b.HasOne("Tracker.Analytics.Db.Models.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_instructions_users_creator_id");

                    b.HasOne("Tracker.Analytics.Db.Models.User", "Executor")
                        .WithMany()
                        .HasForeignKey("ExecutorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_instructions_users_executor_id");

                    b.Navigation("Creator");

                    b.Navigation("Executor");
                });
#pragma warning restore 612, 618
        }
    }
}
