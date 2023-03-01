using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoards.Migrations
{
    /// <inheritdoc />
    public partial class MessageAlbanianUserView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW View_MessageAlbanianUsers AS
            SELECT u.FullName, u.Email, a.Country, c.Message
            FROM Users u, Comments c, Addresses a
            WHERE u.Id = a.UserId
	        AND a.Country = 'Albania'
	        AND u.Id = c.AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DROP VIEW View_MessageAlbanianUsers
            ");
        }
    }
}
