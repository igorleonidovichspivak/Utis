using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utis.Tasks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tasks",
                table: "tasks");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "tasks",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "tasks",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "tasks",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tasks",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "tasks",
                newName: "due_date");

            migrationBuilder.RenameIndex(
                name: "IX_tasks_Status",
                table: "tasks",
                newName: "ix_tasks_status");

            migrationBuilder.RenameIndex(
                name: "IX_tasks_DueDate",
                table: "tasks",
                newName: "ix_tasks_due_date");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tasks",
                table: "tasks",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_tasks",
                table: "tasks");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "tasks",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "tasks",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "tasks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "tasks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "due_date",
                table: "tasks",
                newName: "DueDate");

            migrationBuilder.RenameIndex(
                name: "ix_tasks_status",
                table: "tasks",
                newName: "IX_tasks_Status");

            migrationBuilder.RenameIndex(
                name: "ix_tasks_due_date",
                table: "tasks",
                newName: "IX_tasks_DueDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tasks",
                table: "tasks",
                column: "Id");
        }
    }
}
