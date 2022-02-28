﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tracker.Web.Db;

#nullable disable

namespace Tracker.Web.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("Tracker.Web.Domain.Instruction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CreatorId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ExecDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("ExecutorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("ExecutorId");

                    b.HasIndex("ParentId");

                    b.ToTable("Instructions");
                });

            modelBuilder.Entity("Tracker.Web.Domain.OrgStructElement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar");

                    b.Property<int?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("OrgStruct");
                });

            modelBuilder.Entity("Tracker.Web.Domain.Instruction", b =>
                {
                    b.HasOne("Tracker.Web.Domain.OrgStructElement", null)
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tracker.Web.Domain.OrgStructElement", null)
                        .WithMany()
                        .HasForeignKey("ExecutorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tracker.Web.Domain.Instruction", null)
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Tracker.Web.Domain.OrgStructElement", b =>
                {
                    b.HasOne("Tracker.Web.Domain.OrgStructElement", null)
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Tracker.Web.Domain.Instruction", b =>
                {
                    b.Navigation("Children");
                });
#pragma warning restore 612, 618
        }
    }
}
