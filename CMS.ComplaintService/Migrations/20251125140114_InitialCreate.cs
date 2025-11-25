using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.ComplaintService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplaintAuditHistories",
                columns: table => new
                {
                    AuditID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComplaintID = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintAuditHistories", x => x.AuditID);
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    ComplaintID = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantID = table.Column<string>(type: "TEXT", nullable: false),
                    ConsumerID = table.Column<Guid>(type: "TEXT", nullable: false),
                    AgentID = table.Column<Guid>(type: "TEXT", nullable: true),
                    SupportPersonID = table.Column<Guid>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.ComplaintID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintAuditHistories_ComplaintID",
                table: "ComplaintAuditHistories",
                column: "ComplaintID");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_TenantID_Status",
                table: "Complaints",
                columns: new[] { "TenantID", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplaintAuditHistories");

            migrationBuilder.DropTable(
                name: "Complaints");
        }
    }
}
