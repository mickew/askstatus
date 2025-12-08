using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Askstatus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSensors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SensorType = table.Column<int>(type: "INTEGER", nullable: false),
                    FormatString = table.Column<string>(type: "TEXT", nullable: false),
                    SensorName = table.Column<string>(type: "TEXT", nullable: false),
                    ValueName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_SensorName_ValueName",
                table: "Sensors",
                columns: new[] { "SensorName", "ValueName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sensors");
        }
    }
}
