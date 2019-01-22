﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ASR.Migrations
{
    public partial class removeSlotTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Slot",
                table: "Slot");

            migrationBuilder.DropColumn(
                name: "SlotDate",
                table: "Slot");

            migrationBuilder.RenameColumn(
                name: "SlotTime",
                table: "Slot",
                newName: "StartTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Slot",
                table: "Slot",
                columns: new[] { "RoomName", "StartTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Slot",
                table: "Slot");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Slot",
                newName: "SlotTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "SlotDate",
                table: "Slot",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Slot",
                table: "Slot",
                columns: new[] { "RoomName", "SlotDate", "SlotTime" });
        }
    }
}
