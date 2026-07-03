using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWU.Nagarik.Api.Migrations
{
    /// <inheritdoc />
    public partial class FlattenTranscriptTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transcripts_Institutions_InstitutionId",
                table: "Transcripts");

            migrationBuilder.DropTable(
                name: "Institutions");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Semesters");

            migrationBuilder.DropIndex(
                name: "IX_Transcripts_InstitutionId",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Transcripts");

            migrationBuilder.AddColumn<string>(
                name: "AcademicYearName",
                table: "Transcripts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "CGPA",
                table: "Transcripts",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CollegeName",
                table: "Transcripts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CourseType",
                table: "Transcripts",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditHours",
                table: "Transcripts",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ExamRollNo",
                table: "Transcripts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FacultyName",
                table: "Transcripts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Grade",
                table: "Transcripts",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "GradePoint",
                table: "Transcripts",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GradeValue",
                table: "Transcripts",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Part",
                table: "Transcripts",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProgramName",
                table: "Transcripts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SemesterName",
                table: "Transcripts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SemesterNumber",
                table: "Transcripts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Transcripts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "Transcripts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectCode",
                table: "Transcripts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubjectName",
                table: "Transcripts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Transcripts",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcademicYearName",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "CGPA",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "CollegeName",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "CourseType",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "CreditHours",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "ExamRollNo",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "FacultyName",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "GradePoint",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "GradeValue",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "Part",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "ProgramName",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "SemesterName",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "SemesterNumber",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "SubjectCode",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "SubjectName",
                table: "Transcripts");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Transcripts");

            migrationBuilder.AddColumn<int>(
                name: "InstitutionId",
                table: "Transcripts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Institutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentSerialNo = table.Column<int>(type: "int", nullable: false),
                    DocumentTitle = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LogoPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OfficeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TranscriptId = table.Column<int>(type: "int", nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExamRollNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SemesterNumber = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Semesters_Transcripts_TranscriptId",
                        column: x => x.TranscriptId,
                        principalTable: "Transcripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemesterId = table.Column<int>(type: "int", nullable: false),
                    CourseType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreditHours = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    GradePoint = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    GradeValue = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    SubjectCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubjectName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transcripts_InstitutionId",
                table: "Transcripts",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_IsActive",
                table: "Institutions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Semesters_TranscriptId",
                table: "Semesters",
                column: "TranscriptId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SemesterId",
                table: "Subjects",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transcripts_Institutions_InstitutionId",
                table: "Transcripts",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id");
        }
    }
}
