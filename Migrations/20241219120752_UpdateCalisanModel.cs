using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kuafor.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCalisanModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UzmanlikAlanlari",
                table: "Calisanlar");

            migrationBuilder.AlterColumn<string>(
                name: "Soyad",
                table: "Calisanlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Ad",
                table: "Calisanlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "UzmanlikHizmetId",
                table: "Calisanlar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calisanlar_UzmanlikHizmetId",
                table: "Calisanlar",
                column: "UzmanlikHizmetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calisanlar_Hizmetler_UzmanlikHizmetId",
                table: "Calisanlar",
                column: "UzmanlikHizmetId",
                principalTable: "Hizmetler",
                principalColumn: "HizmetId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calisanlar_Hizmetler_UzmanlikHizmetId",
                table: "Calisanlar");

            migrationBuilder.DropIndex(
                name: "IX_Calisanlar_UzmanlikHizmetId",
                table: "Calisanlar");

            migrationBuilder.DropColumn(
                name: "UzmanlikHizmetId",
                table: "Calisanlar");

            migrationBuilder.AlterColumn<string>(
                name: "Soyad",
                table: "Calisanlar",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Ad",
                table: "Calisanlar",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "UzmanlikAlanlari",
                table: "Calisanlar",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
