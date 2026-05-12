using Microsoft.EntityFrameworkCore.Migrations;


namespace EventApp.Data.Migrations
{
    public partial class AddEventCategoryAndFormat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Events",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "meetup");

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "Events",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "offline");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "Events");
        }
    }
}
