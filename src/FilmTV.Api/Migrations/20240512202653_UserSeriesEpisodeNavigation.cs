using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmTV.Api.Migrations
{
    /// <inheritdoc />
    public partial class UserSeriesEpisodeNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEpisodes_UserSeries_UserSeriesSeriesId",
                table: "UserEpisodes");

            migrationBuilder.DropIndex(
                name: "IX_UserEpisodes_UserSeriesSeriesId",
                table: "UserEpisodes");

            migrationBuilder.DropColumn(
                name: "UserSeriesSeriesId",
                table: "UserEpisodes");

            migrationBuilder.AddColumn<int>(
                name: "SeriesId",
                table: "UserEpisodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserEpisodes_SeriesId",
                table: "UserEpisodes",
                column: "SeriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEpisodes_UserSeries_SeriesId",
                table: "UserEpisodes",
                column: "SeriesId",
                principalTable: "UserSeries",
                principalColumn: "SeriesId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEpisodes_UserSeries_SeriesId",
                table: "UserEpisodes");

            migrationBuilder.DropIndex(
                name: "IX_UserEpisodes_SeriesId",
                table: "UserEpisodes");

            migrationBuilder.DropColumn(
                name: "SeriesId",
                table: "UserEpisodes");

            migrationBuilder.AddColumn<int>(
                name: "UserSeriesSeriesId",
                table: "UserEpisodes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserEpisodes_UserSeriesSeriesId",
                table: "UserEpisodes",
                column: "UserSeriesSeriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEpisodes_UserSeries_UserSeriesSeriesId",
                table: "UserEpisodes",
                column: "UserSeriesSeriesId",
                principalTable: "UserSeries",
                principalColumn: "SeriesId");
        }
    }
}
