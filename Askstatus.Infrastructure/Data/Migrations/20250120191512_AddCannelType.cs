using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Askstatus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCannelType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChanelType",
                table: "PowerDevices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChanelType",
                table: "PowerDevices");
        }
    }
}
