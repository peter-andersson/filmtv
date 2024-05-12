using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmTV.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeShowToSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Shows_ShowId",
                table: "Episodes");

            migrationBuilder.DropTable(
                name: "UserShows");

            migrationBuilder.DropTable(
                name: "Shows");

            migrationBuilder.RenameColumn(
                name: "ShowId",
                table: "Episodes",
                newName: "SeriesId");

            migrationBuilder.RenameIndex(
                name: "IX_Episodes_ShowId",
                table: "Episodes",
                newName: "IX_Episodes_SeriesId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserEpisodes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserSeriesSeriesId",
                table: "UserEpisodes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ImdbId = table.Column<string>(type: "text", nullable: false),
                    TvDbId = table.Column<int>(type: "integer", nullable: false),
                    OriginalTitle = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ETag = table.Column<string>(type: "text", nullable: false),
                    NextUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSeries",
                columns: table => new
                {
                    SeriesId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    RatingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSeries", x => x.SeriesId);
                    table.ForeignKey(
                        name: "FK_UserSeries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSeries_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEpisodes_UserId",
                table: "UserEpisodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEpisodes_UserSeriesSeriesId",
                table: "UserEpisodes",
                column: "UserSeriesSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSeries_UserId",
                table: "UserSeries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Series_SeriesId",
                table: "Episodes",
                column: "SeriesId",
                principalTable: "Series",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEpisodes_AspNetUsers_UserId",
                table: "UserEpisodes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEpisodes_UserSeries_UserSeriesSeriesId",
                table: "UserEpisodes",
                column: "UserSeriesSeriesId",
                principalTable: "UserSeries",
                principalColumn: "SeriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Episodes_Series_SeriesId",
                table: "Episodes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEpisodes_AspNetUsers_UserId",
                table: "UserEpisodes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEpisodes_UserSeries_UserSeriesSeriesId",
                table: "UserEpisodes");

            migrationBuilder.DropTable(
                name: "UserSeries");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropIndex(
                name: "IX_UserEpisodes_UserId",
                table: "UserEpisodes");

            migrationBuilder.DropIndex(
                name: "IX_UserEpisodes_UserSeriesSeriesId",
                table: "UserEpisodes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserEpisodes");

            migrationBuilder.DropColumn(
                name: "UserSeriesSeriesId",
                table: "UserEpisodes");

            migrationBuilder.RenameColumn(
                name: "SeriesId",
                table: "Episodes",
                newName: "ShowId");

            migrationBuilder.RenameIndex(
                name: "IX_Episodes_SeriesId",
                table: "Episodes",
                newName: "IX_Episodes_ShowId");

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ETag = table.Column<string>(type: "text", nullable: false),
                    ImdbId = table.Column<string>(type: "text", nullable: false),
                    NextUpdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OriginalTitle = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TvDbId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserShows",
                columns: table => new
                {
                    ShowId = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    RatingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserShows", x => x.ShowId);
                    table.ForeignKey(
                        name: "FK_UserShows_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Episodes_Shows_ShowId",
                table: "Episodes",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
