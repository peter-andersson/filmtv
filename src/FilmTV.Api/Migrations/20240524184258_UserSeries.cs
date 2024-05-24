using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilmTV.Api.Migrations
{
    /// <inheritdoc />
    public partial class UserSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEpisodes_UserSeries_SeriesId",
                table: "UserEpisodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSeries",
                table: "UserSeries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEpisodes",
                table: "UserEpisodes");

            migrationBuilder.DropIndex(
                name: "IX_UserEpisodes_SeriesId",
                table: "UserEpisodes");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "UserSeries",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserSeriesSeriesId",
                table: "UserEpisodes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserSeriesUserId",
                table: "UserEpisodes",
                type: "character varying(50)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Series",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalTitle",
                table: "Series",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ImdbId",
                table: "Series",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ETag",
                table: "Series",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Episodes",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSeries",
                table: "UserSeries",
                columns: new[] { "SeriesId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEpisodes",
                table: "UserEpisodes",
                columns: new[] { "EpisodeId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserEpisodes_SeriesId_UserId",
                table: "UserEpisodes",
                columns: new[] { "SeriesId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserEpisodes_UserSeriesSeriesId_UserSeriesUserId",
                table: "UserEpisodes",
                columns: new[] { "UserSeriesSeriesId", "UserSeriesUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserEpisodes_UserSeries_SeriesId_UserId",
                table: "UserEpisodes",
                columns: new[] { "SeriesId", "UserId" },
                principalTable: "UserSeries",
                principalColumns: new[] { "SeriesId", "UserId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEpisodes_UserSeries_UserSeriesSeriesId_UserSeriesUserId",
                table: "UserEpisodes",
                columns: new[] { "UserSeriesSeriesId", "UserSeriesUserId" },
                principalTable: "UserSeries",
                principalColumns: new[] { "SeriesId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEpisodes_UserSeries_SeriesId_UserId",
                table: "UserEpisodes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserEpisodes_UserSeries_UserSeriesSeriesId_UserSeriesUserId",
                table: "UserEpisodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSeries",
                table: "UserSeries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEpisodes",
                table: "UserEpisodes");

            migrationBuilder.DropIndex(
                name: "IX_UserEpisodes_SeriesId_UserId",
                table: "UserEpisodes");

            migrationBuilder.DropIndex(
                name: "IX_UserEpisodes_UserSeriesSeriesId_UserSeriesUserId",
                table: "UserEpisodes");

            migrationBuilder.DropColumn(
                name: "UserSeriesSeriesId",
                table: "UserEpisodes");

            migrationBuilder.DropColumn(
                name: "UserSeriesUserId",
                table: "UserEpisodes");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "UserSeries",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Series",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalTitle",
                table: "Series",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "ImdbId",
                table: "Series",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ETag",
                table: "Series",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Episodes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSeries",
                table: "UserSeries",
                column: "SeriesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEpisodes",
                table: "UserEpisodes",
                column: "EpisodeId");

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
    }
}
