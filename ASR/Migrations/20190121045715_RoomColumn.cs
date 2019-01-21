using Microsoft.EntityFrameworkCore.Migrations;

namespace ASR.Migrations
{
    public partial class RoomColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FName",
                table: "Room",
                newName: "RoomID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoomID",
                table: "Room",
                newName: "FName");
        }
    }
}
