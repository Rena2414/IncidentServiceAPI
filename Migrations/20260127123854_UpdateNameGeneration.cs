using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IncidentServiceAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNameGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IncidentName",
                table: "Incidents",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValueSql: "lower(replace(newid(), '-', ''))",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IncidentName",
                table: "Incidents",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldDefaultValueSql: "lower(replace(newid(), '-', ''))");
        }
    }
}
