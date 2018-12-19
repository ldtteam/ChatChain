using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServerWebApp.Migrations
{
    public partial class groupId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "Groups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Groups");
        }
    }
}
