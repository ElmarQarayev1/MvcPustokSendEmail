using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcPustok.Migrations
{
    /// <inheritdoc />
    public partial class AlterTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookImages_Books_BooksId",
                table: "BookImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorsId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Genres_GenresId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTags_Books_BooksId",
                table: "BookTags");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTags_Tags_TagsId",
                table: "BookTags");

            migrationBuilder.DropIndex(
                name: "IX_BookTags_BooksId",
                table: "BookTags");

            migrationBuilder.DropIndex(
                name: "IX_BookTags_TagsId",
                table: "BookTags");

            migrationBuilder.DropIndex(
                name: "IX_Books_AuthorsId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_GenresId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_BookImages_BooksId",
                table: "BookImages");

            migrationBuilder.DropColumn(
                name: "BooksId",
                table: "BookTags");

            migrationBuilder.DropColumn(
                name: "TagsId",
                table: "BookTags");

            migrationBuilder.DropColumn(
                name: "AuthorsId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "GenresId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BooksId",
                table: "BookImages");

            migrationBuilder.DropColumn(
                name: "PosterStatus",
                table: "BookImages");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Authors",
                newName: "Fullname");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tags",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Tags",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Genres",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Genres",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Genres",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BookTags",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "BookTags",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SalePrice",
                table: "Books",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Books",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "Books",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostPrice",
                table: "Books",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Books",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNew",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Books",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BookImages",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BookImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "BookImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "BookImages",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Fullname",
                table: "Authors",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Authors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Authors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookTags_BookId",
                table: "BookTags",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTags_TagId",
                table: "BookTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_GenreId",
                table: "Books",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_BookImages_BookId",
                table: "BookImages",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookImages_Books_BookId",
                table: "BookImages",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Genres_GenreId",
                table: "Books",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTags_Books_BookId",
                table: "BookTags",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTags_Tags_TagId",
                table: "BookTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookImages_Books_BookId",
                table: "BookImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Genres_GenreId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTags_Books_BookId",
                table: "BookTags");

            migrationBuilder.DropForeignKey(
                name: "FK_BookTags_Tags_TagId",
                table: "BookTags");

            migrationBuilder.DropIndex(
                name: "IX_BookTags_BookId",
                table: "BookTags");

            migrationBuilder.DropIndex(
                name: "IX_BookTags_TagId",
                table: "BookTags");

            migrationBuilder.DropIndex(
                name: "IX_Books_AuthorId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_GenreId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_BookImages_BookId",
                table: "BookImages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BookTags");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "BookTags");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BookImages");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "BookImages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BookImages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Authors");

            migrationBuilder.RenameColumn(
                name: "Fullname",
                table: "Authors",
                newName: "FullName");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Genres",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "BooksId",
                table: "BookTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TagsId",
                table: "BookTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "SalePrice",
                table: "Books",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Desc",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<decimal>(
                name: "CostPrice",
                table: "Books",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AddColumn<int>(
                name: "AuthorsId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GenresId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BookImages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<int>(
                name: "BooksId",
                table: "BookImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PosterStatus",
                table: "BookImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.CreateIndex(
                name: "IX_BookTags_BooksId",
                table: "BookTags",
                column: "BooksId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTags_TagsId",
                table: "BookTags",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorsId",
                table: "Books",
                column: "AuthorsId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_GenresId",
                table: "Books",
                column: "GenresId");

            migrationBuilder.CreateIndex(
                name: "IX_BookImages_BooksId",
                table: "BookImages",
                column: "BooksId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookImages_Books_BooksId",
                table: "BookImages",
                column: "BooksId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorsId",
                table: "Books",
                column: "AuthorsId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Genres_GenresId",
                table: "Books",
                column: "GenresId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTags_Books_BooksId",
                table: "BookTags",
                column: "BooksId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookTags_Tags_TagsId",
                table: "BookTags",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
