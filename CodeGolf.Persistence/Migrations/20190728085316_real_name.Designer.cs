﻿// <auto-generated />
using System;
using CodeGolf.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CodeGolf.Persistence.Migrations
{
    [DbContext(typeof(CodeGolfContext))]
    [Migration("20190728085316_real_name")]
    partial class real_name
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("CodeGolf.Domain.Attempt", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired();

                    b.Property<Guid>("HoleId");

                    b.Property<int>("Score");

                    b.Property<DateTime>("TimeStamp");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("HoleId");

                    b.HasIndex("UserId");

                    b.ToTable("Attempts");
                });

            modelBuilder.Entity("CodeGolf.Domain.HoleInstance", b =>
                {
                    b.Property<Guid>("HoleId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("End");

                    b.Property<DateTime>("Start");

                    b.HasKey("HoleId");

                    b.ToTable("Holes");
                });

            modelBuilder.Entity("CodeGolf.Domain.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AvatarUri")
                        .IsRequired();

                    b.Property<string>("LoginName")
                        .IsRequired();

                    b.Property<string>("RealName")
                        .IsRequired();

                    b.HasKey("UserId");

                    b.HasIndex("LoginName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CodeGolf.Domain.Attempt", b =>
                {
                    b.HasOne("CodeGolf.Domain.HoleInstance")
                        .WithMany()
                        .HasForeignKey("HoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CodeGolf.Domain.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
