using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TMWalks.API.Migrations
{
    /// <inheritdoc />
    public partial class AddingImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("196ac010-77f7-4064-bc70-793be6f43e38"));

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("47a8f078-809d-4f70-9cf4-82f1c583daa5"));

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("525ffa3f-b521-4de5-be46-32075b2dff4c"));

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Difficulties",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0440aa63-1a94-4a03-bb71-73d6ab8b0f91"), "Hard" },
                    { new Guid("c3e9bbf2-5558-4bff-b6c7-bb93560b3e9f"), "Medium" },
                    { new Guid("f6493a41-c5d2-49da-8522-8348df063086"), "Easy" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("0440aa63-1a94-4a03-bb71-73d6ab8b0f91"));

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("c3e9bbf2-5558-4bff-b6c7-bb93560b3e9f"));

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: new Guid("f6493a41-c5d2-49da-8522-8348df063086"));

            migrationBuilder.InsertData(
                table: "Difficulties",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("196ac010-77f7-4064-bc70-793be6f43e38"), "Medium" },
                    { new Guid("47a8f078-809d-4f70-9cf4-82f1c583daa5"), "Hard" },
                    { new Guid("525ffa3f-b521-4de5-be46-32075b2dff4c"), "Easy" }
                });
        }
    }
}
