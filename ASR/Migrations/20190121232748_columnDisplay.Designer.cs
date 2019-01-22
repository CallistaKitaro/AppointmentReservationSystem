﻿// <auto-generated />
using System;
using ASR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ASR.Migrations
{
    [DbContext(typeof(ASRContext))]
    [Migration("20190121232748_columnDisplay")]
    partial class columnDisplay
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ASR.Models.Admin", b =>
                {
                    b.Property<string>("AdminID");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.HasKey("AdminID");

                    b.ToTable("Admin");
                });

            modelBuilder.Entity("ASR.Models.Room", b =>
                {
                    b.Property<string>("RoomID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RoomName")
                        .IsRequired()
                        .HasMaxLength(1);

                    b.HasKey("RoomID");

                    b.ToTable("Room");
                });

            modelBuilder.Entity("ASR.Models.Slot", b =>
                {
                    b.Property<string>("RoomID")
                        .HasColumnName("RoomID");

                    b.Property<DateTime>("StartTime")
                        .HasColumnName("StartTime");

                    b.Property<string>("StaffID")
                        .IsRequired();

                    b.Property<string>("StudentID")
                        .HasColumnName("BookedInStudentID");

                    b.HasKey("RoomID", "StartTime");

                    b.HasIndex("StaffID");

                    b.HasIndex("StudentID");

                    b.ToTable("Slot");
                });

            modelBuilder.Entity("ASR.Models.Staff", b =>
                {
                    b.Property<string>("StaffID");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.HasKey("StaffID");

                    b.ToTable("Staff");
                });

            modelBuilder.Entity("ASR.Models.Student", b =>
                {
                    b.Property<string>("StudentID");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.HasKey("StudentID");

                    b.ToTable("Student");
                });

            modelBuilder.Entity("ASR.Models.Slot", b =>
                {
                    b.HasOne("ASR.Models.Room", "Room")
                        .WithMany("Slots")
                        .HasForeignKey("RoomID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ASR.Models.Staff", "Staff")
                        .WithMany("StaffSlots")
                        .HasForeignKey("StaffID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ASR.Models.Student", "Student")
                        .WithMany("StudentSlots")
                        .HasForeignKey("StudentID");
                });
#pragma warning restore 612, 618
        }
    }
}
