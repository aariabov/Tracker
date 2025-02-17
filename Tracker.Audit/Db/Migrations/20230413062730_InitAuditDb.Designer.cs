﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tracker.Audit.Db;

#nullable disable

namespace Tracker.Audit.Db.Migrations
{
    [DbContext(typeof(AuditDbContext))]
    [Migration("20230413062730_InitAuditDb")]
    partial class InitAuditDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tracker.Audit.Db.Models.AuditLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_time");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("entity_id");

                    b.Property<string>("EntityName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("entity_name");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_audit_logs");

                    b.ToTable("audit_logs", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
