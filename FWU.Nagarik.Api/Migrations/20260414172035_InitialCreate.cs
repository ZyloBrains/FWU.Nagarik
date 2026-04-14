using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWU.Nagarik.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentMarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegdNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubjectName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubjectCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Marks = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Semester = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AcademicYear = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentMarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    RegdNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DobAD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProgramName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IntakeYear = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    School = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CgpaScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    GraduateYear = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.RegdNo);
                });

            migrationBuilder.CreateTable(
                name: "VerificationLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegdNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DobAD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProgramName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IntakeYear = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    School = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CgpaScore = table.Column<double>(type: "float", nullable: true),
                    GraduateYear = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentMarks_RegdNo",
                table: "StudentMarks",
                column: "RegdNo");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationLogs_RegdNo",
                table: "VerificationLogs",
                column: "RegdNo");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationLogs_VerifiedAt",
                table: "VerificationLogs",
                column: "VerifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentMarks");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "VerificationLogs");
        }
    }
}
