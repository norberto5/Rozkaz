﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Rozkaz.Models;

namespace Rozkaz.Migrations
{
    [DbContext(typeof(RozkazDatabaseContext))]
    [Migration("20190603205310_AddedUnitModelToUser")]
    partial class AddedUnitModelToUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Rozkaz.Models.UnitModel", b =>
                {
                    b.Property<Guid>("Uid")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NameFirstLine");

                    b.Property<string>("NameSecondLine");

                    b.Property<string>("SubtextLines");

                    b.HasKey("Uid");

                    b.ToTable("UnitModel");
                });

            modelBuilder.Entity("Rozkaz.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DisplayName");

                    b.Property<string>("Mail");

                    b.Property<string>("Name");

                    b.Property<Guid?>("UnitUid");

                    b.HasKey("Id");

                    b.HasIndex("UnitUid");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Rozkaz.Models.User", b =>
                {
                    b.HasOne("Rozkaz.Models.UnitModel", "Unit")
                        .WithMany()
                        .HasForeignKey("UnitUid");
                });
#pragma warning restore 612, 618
        }
    }
}
