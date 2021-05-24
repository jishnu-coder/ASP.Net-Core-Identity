using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityAuth.Migrations
{
    public partial class Dataad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "customers",
                columns: new[] { "Id", "Gender", "Name" },
                values: new object[,]
                {
                    { 1, "Male", "Sam" },
                    { 2, "Female", "Alexa" },
                    { 3, "Male", "Krish" },
                    { 4, "Female", "Zaya" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "customers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "customers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "customers",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
