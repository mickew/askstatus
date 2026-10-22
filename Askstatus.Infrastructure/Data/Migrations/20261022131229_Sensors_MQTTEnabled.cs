using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Askstatus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Sensors_MQTTEnabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MQTTEnabled",
                table: "Sensors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MQTTEnabled",
                table: "Sensors");
        }
    }
}
