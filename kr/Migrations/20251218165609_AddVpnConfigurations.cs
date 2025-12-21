using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace demo.Migrations
{
    /// <inheritdoc />
    public partial class AddVpnConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VpnConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ServerAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ServerPort = table.Column<int>(type: "INTEGER", nullable: false),
                    Protocol = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CertificateAuthority = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    ClientCertificate = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    ClientKey = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    UseCompression = table.Column<bool>(type: "INTEGER", nullable: false),
                    RedirectGateway = table.Column<bool>(type: "INTEGER", nullable: false),
                    DnsServers = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnConfigurations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VpnConfigurations_Name",
                table: "VpnConfigurations",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VpnConfigurations");
        }
    }
}
