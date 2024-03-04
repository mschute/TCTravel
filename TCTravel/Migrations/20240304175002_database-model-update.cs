using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TCTravel.Migrations
{
    /// <inheritdoc />
    public partial class databasemodelupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientCompanyId",
                table: "Customers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ClientCompanyId",
                table: "Customers",
                column: "ClientCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_ClientCompanies_ClientCompanyId",
                table: "Customers",
                column: "ClientCompanyId",
                principalTable: "ClientCompanies",
                principalColumn: "ClientCompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_ClientCompanies_ClientCompanyId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ClientCompanyId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ClientCompanyId",
                table: "Customers");
        }
    }
}
