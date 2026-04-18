using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWU.Nagarik.Api.Migrations
{
    /// <inheritdoc />
    public partial class StudentRequestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Requestedby = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentAdmissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentRequests_Students_StudentAdmissionId",
                        column: x => x.StudentAdmissionId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentRequests_StudentAdmissionId",
                table: "StudentRequests",
                column: "StudentAdmissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentRequests");
        }
    }
}
