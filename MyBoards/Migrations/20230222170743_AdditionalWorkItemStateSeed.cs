using Microsoft.EntityFrameworkCore.Migrations;
using MyBoards.Entities;

#nullable disable

namespace MyBoards.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalWorkItemStateSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(table: "WorkItemsStates", column: "Value", value: "On hold");
            migrationBuilder.InsertData(table: "WorkItemsStates", column: "Value", value: "Rejected");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "WorkItemsStates", keyColumn: "Value", keyValue: "Rejected");
            migrationBuilder.DeleteData(table: "WorkItemsStates", keyColumn: "Value", keyValue: "On hold");

        }
    }
}
