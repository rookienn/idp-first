using Microsoft.EntityFrameworkCore.Migrations;

namespace is4FirstDemo.Data.Migrations.IdentityServer.UserDb
{
    public partial class InitialIdentityServerIdentityUserDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdentityUser",
                columns: table => new
                {
                    SubjectId = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    ProviderName = table.Column<string>(nullable: true),
                    ProviderSubjectId = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IdentityUserClaims = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "IdentityUserClaim",
                columns: table => new
                {
                    ClaimId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    UserSubjectId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaim", x => x.ClaimId);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaim_IdentityUser_UserSubjectId",
                        column: x => x.UserSubjectId,
                        principalTable: "IdentityUser",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdentityUserClaim_UserSubjectId",
                table: "IdentityUserClaim",
                column: "UserSubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityUserClaim");

            migrationBuilder.DropTable(
                name: "IdentityUser");
        }
    }
}
