using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FWU.Nagarik.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTranscriptUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transcripts_IssueSerialNo",
                table: "Transcripts");

            migrationBuilder.CreateIndex(
                name: "IX_Transcripts_RegdNo_IssueSerialNo_SubjectCode",
                table: "Transcripts",
                columns: new[] { "RegdNo", "IssueSerialNo", "SubjectCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transcripts_RegdNo_IssueSerialNo_SubjectCode",
                table: "Transcripts");

            migrationBuilder.CreateIndex(
                name: "IX_Transcripts_IssueSerialNo",
                table: "Transcripts",
                column: "IssueSerialNo",
                unique: true);
        }
    }
}
