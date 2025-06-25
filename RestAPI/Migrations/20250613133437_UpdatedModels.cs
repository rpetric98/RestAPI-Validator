using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Carriers_CarrierId",
                table: "Flights");

            migrationBuilder.DropTable(
                name: "Carriers");

            migrationBuilder.DropIndex(
                name: "IX_Flights_CarrierId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "CarrierId",
                table: "Flights");

            migrationBuilder.AddColumn<string>(
                name: "Carrier_DisplayCode",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Carrier_Logo",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Carrier_Name",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Carrier_DisplayCode",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "Carrier_Logo",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "Carrier_Name",
                table: "Flights");

            migrationBuilder.AddColumn<string>(
                name: "CarrierId",
                table: "Flights",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Carriers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carriers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flights_CarrierId",
                table: "Flights",
                column: "CarrierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Carriers_CarrierId",
                table: "Flights",
                column: "CarrierId",
                principalTable: "Carriers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
