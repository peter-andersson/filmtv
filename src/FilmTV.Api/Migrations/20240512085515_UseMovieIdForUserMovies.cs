using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FilmTV.Api.Migrations
{
    /// <inheritdoc />
    public partial class UseMovieIdForUserMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMovies",
                table: "UserMovies");

            migrationBuilder.DropIndex(
                name: "IX_UserMovies_MovieId",
                table: "UserMovies");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserMovies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMovies",
                table: "UserMovies",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMovies",
                table: "UserMovies");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserMovies",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMovies",
                table: "UserMovies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserMovies_MovieId",
                table: "UserMovies",
                column: "MovieId");
        }
    }
}
