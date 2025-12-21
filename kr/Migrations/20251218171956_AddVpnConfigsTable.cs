using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace demo.Migrations
{
    /// <inheritdoc />
    public partial class AddVpnConfigsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VpnConfigurations");

            migrationBuilder.CreateTable(
                name: "VpnConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ConfigType = table.Column<string>(type: "TEXT", nullable: false),
                    ServerAddress = table.Column<string>(type: "TEXT", nullable: false),
                    ServerPort = table.Column<int>(type: "INTEGER", nullable: false),
                    Protocol = table.Column<string>(type: "TEXT", nullable: false),
                    ConfigContent = table.Column<string>(type: "TEXT", nullable: false),
                    CaCertificate = table.Column<string>(type: "TEXT", nullable: false),
                    ClientCertificate = table.Column<string>(type: "TEXT", nullable: false),
                    ClientKey = table.Column<string>(type: "TEXT", nullable: false),
                    TlsAuthKey = table.Column<string>(type: "TEXT", nullable: false),
                    DhParameters = table.Column<string>(type: "TEXT", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Cipher = table.Column<string>(type: "TEXT", nullable: false),
                    Auth = table.Column<string>(type: "TEXT", nullable: false),
                    Mtu = table.Column<int>(type: "INTEGER", nullable: false),
                    RedirectGateway = table.Column<bool>(type: "INTEGER", nullable: false),
                    BlockDns = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdditionalOptions = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnConfigs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VpnConfigs_IsDefault",
                table: "VpnConfigs",
                column: "IsDefault",
                filter: "IsDefault = 1");

            migrationBuilder.CreateIndex(
                name: "IX_VpnConfigs_Name",
                table: "VpnConfigs",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VpnConfigs");

            migrationBuilder.CreateTable(
                name: "VpnConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CertificateAuthority = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    ClientCertificate = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    ClientKey = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DnsServers = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Protocol = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RedirectGateway = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ServerPort = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UseCompression = table.Column<bool>(type: "INTEGER", nullable: false)
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
    }
}
