using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApiApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddedProfilePhotoColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "profilePhoto",
                table: "ChatUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profilePhoto",
                table: "ChatUsers");
        }
    }
}
