using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektAWiMDDD.Migrations
{
    /// <inheritdoc />
    public partial class sessionFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlannedReps",
                table: "WorkoutSessionExercises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedSets",
                table: "WorkoutSessionExercises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PlannedWeight",
                table: "WorkoutSessionExercises",
                type: "decimal(7,2)",
                precision: 7,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlannedReps",
                table: "WorkoutSessionExercises");

            migrationBuilder.DropColumn(
                name: "PlannedSets",
                table: "WorkoutSessionExercises");

            migrationBuilder.DropColumn(
                name: "PlannedWeight",
                table: "WorkoutSessionExercises");
        }
    }
}
