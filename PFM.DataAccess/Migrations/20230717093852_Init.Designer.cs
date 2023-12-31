﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PFM.DataAccess.DbContextOption;

#nullable disable

namespace PFM.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230717093852_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PFM.DataAccess.Entities.Category", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ParentCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Code");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("PFM.DataAccess.Entities.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<double>("Ammount")
                        .HasColumnType("double precision");

                    b.Property<string>("BeneficiaryName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CatCode")
                        .HasColumnType("text");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Direction")
                        .HasColumnType("integer");

                    b.Property<int>("Kind")
                        .HasColumnType("integer");

                    b.Property<int?>("Mcc")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CatCode");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("PFM.DataAccess.Entities.Transaction", b =>
                {
                    b.HasOne("PFM.DataAccess.Entities.Category", "Category")
                        .WithMany("Transactions")
                        .HasForeignKey("CatCode");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("PFM.DataAccess.Entities.Category", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
