using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Operational_Risk_Management.Migrations
{
    /// <inheritdoc />
    public partial class user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    KriSubmissionAccess = table.Column<bool>(type: "bit", nullable: false),
                    Phone1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByFullName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUsers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Incidents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchDepartmentUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscoveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscoveredBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleOfIncident = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityProcess = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstLineRiskOwner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncidentType = table.Column<int>(type: "int", nullable: false),
                    IncidentStatus = table.Column<int>(type: "int", nullable: false),
                    IncidentCategoryLevel = table.Column<int>(type: "int", nullable: false),
                    RiskType = table.Column<int>(type: "int", nullable: false),
                    RiskAssessment = table.Column<int>(type: "int", nullable: false),
                    FinancialLossCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinancialLossGrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinancialLossLegalCosts = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinancialLossOtherCosts = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinancialLossRecoveryFromClient = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinancialLossRecoveryFromInsurance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinancialLossNetLossToDate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GainCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GainGrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GainLegalCosts = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GainOtherCosts = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GainRecoveryFromClient = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GainRecoveryFromInsurance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GainNetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EventIncidentDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventIncidentCause = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventIncidentDiscovery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectiveActionsImplemented = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasWrittenProcedure = table.Column<bool>(type: "bit", nullable: false),
                    ProcedureIncludesControls = table.Column<bool>(type: "bit", nullable: false),
                    ExistingControlMeasures = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReasonsForFailure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProposedPoliciesProcedures = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProposedControls = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProposedHumanResources = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProposedSystems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProposedOthers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherComments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RiskManagementNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiresRevision = table.Column<bool>(type: "bit", nullable: false),
                    LastReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByFullName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsEmailSent = table.Column<bool>(type: "bit", nullable: false),
                    EmailSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSystemNotification = table.Column<bool>(type: "bit", nullable: false),
                    IsRiskManagementNotification = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });



            migrationBuilder.CreateTable(
                name: "IncidentDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByFullName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentDocuments_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffConcerned",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleFunction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByFullName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffConcerned", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffConcerned_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_DepartmentId",
                table: "ApplicationUsers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentDocuments_IncidentId",
                table: "IncidentDocuments",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffConcerned_IncidentId",
                table: "StaffConcerned",
                column: "IncidentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "IncidentDocuments");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "StaffConcerned");

            migrationBuilder.DropTable(
                name: "Incidents");


        }
    }
}
