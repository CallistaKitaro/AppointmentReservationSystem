using Microsoft.EntityFrameworkCore.Migrations;

namespace ASR.Migrations
{
    public partial class columnDisplay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slot_Room_RoomName",
                table: "Slot");

            migrationBuilder.RenameColumn(
                name: "RoomName",
                table: "Slot",
                newName: "RoomID");

            migrationBuilder.AddForeignKey(
                name: "FK_Slot_Room_RoomID",
                table: "Slot",
                column: "RoomID",
                principalTable: "Room",
                principalColumn: "RoomID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slot_Room_RoomID",
                table: "Slot");

            migrationBuilder.RenameColumn(
                name: "RoomID",
                table: "Slot",
                newName: "RoomName");

            migrationBuilder.AddForeignKey(
                name: "FK_Slot_Room_RoomName",
                table: "Slot",
                column: "RoomName",
                principalTable: "Room",
                principalColumn: "RoomID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
