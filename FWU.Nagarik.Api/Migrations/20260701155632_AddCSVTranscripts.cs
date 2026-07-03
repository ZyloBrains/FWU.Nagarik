using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWU.Nagarik.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCSVTranscripts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.AddColumn<string>(
                name: "ExamRollNo",
                table: "Semesters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamRollNo",
                table: "Semesters");

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlobName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BlobUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CertificateType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProgramName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegdNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_RegdNo",
                table: "Certificates",
                column: "RegdNo");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_RegdNo_ProgramName_CertificateType",
                table: "Certificates",
                columns: new[] { "RegdNo", "ProgramName", "CertificateType" },
                unique: true);
        }
    }
}
