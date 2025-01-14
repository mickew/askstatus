using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Askstatus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPowerDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PowerDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceType = table.Column<int>(type: "INTEGER", nullable: false),
                    HostName = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceName = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceMac = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceModel = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceGen = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerDevices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PowerDevices_DeviceMac",
                table: "PowerDevices",
                column: "DeviceMac",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PowerDevices_HostName",
                table: "PowerDevices",
                column: "HostName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PowerDevices");
        }
    }
}
