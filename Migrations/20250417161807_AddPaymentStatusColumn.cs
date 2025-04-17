using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_NhomSugar_SD19311.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCheckout",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCheckout",
                table: "Carts");
        }
    }
}
