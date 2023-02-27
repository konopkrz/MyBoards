using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoards.Migrations
{
    /// <inheritdoc />
    public partial class ViewTopAuthorsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW View_TopAuthors AS
            SELECT TOP 5 u.FullName, COUNT (*) WorkItemCreated
            FROM Users u
            JOIN WorkItems wi ON wi.AuthorId = u.Id
            GROUP BY u.Id, u.FullName
            ORDER BY WorkItemCreated DESC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DROP VIEW View_TopAuthors
            ");
        }
    }
}
