using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmEazy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatePhoneNumberType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "User",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PhoneNumber",
                table: "User",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14);
        }
    }
}
