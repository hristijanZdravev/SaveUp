using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SaveUp.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubGroup = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_BodyGroups_BodyGroupId",
                        column: x => x.BodyGroupId,
                        principalTable: "BodyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkoutExerciseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SetNumber = table.Column<int>(type: "int", nullable: false),
                    Reps = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DurationSeconds = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSets_WorkoutExercises_WorkoutExerciseId",
                        column: x => x.WorkoutExerciseId,
                        principalTable: "WorkoutExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BodyGroups",
                columns: new[] { "Id", "Name", "SubGroup" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Chest", null },
                    { new Guid("22222222-1111-1111-1111-111111111111"), "Back", null },
                    { new Guid("33333333-1111-1111-1111-111111111111"), "Legs", null },
                    { new Guid("44444444-1111-1111-1111-111111111111"), "Shoulders", null }
                });

            migrationBuilder.InsertData(
                table: "Workouts",
                columns: new[] { "Id", "Date", "Title", "UserId" },
                values: new object[] { new Guid("90000000-0000-0000-0000-000000000001"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Push Day", "user@test.com" });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "BodyGroupId", "Description", "ImageUrl", "Title" },
                values: new object[,]
                {
                    { new Guid("aaaaaaa1-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "Compound chest pressing movement.", null, "Barbell Bench Press" },
                    { new Guid("aaaaaaa2-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111"), "Upper chest focused pressing movement.", null, "Incline Dumbbell Press" },
                    { new Guid("aaaaaaa3-1111-1111-1111-111111111111"), new Guid("22222222-1111-1111-1111-111111111111"), "Bodyweight vertical pulling movement.", null, "Pull Ups" },
                    { new Guid("aaaaaaa4-1111-1111-1111-111111111111"), new Guid("33333333-1111-1111-1111-111111111111"), "Compound lower body movement.", null, "Barbell Squats" },
                    { new Guid("aaaaaaa5-1111-1111-1111-111111111111"), new Guid("44444444-1111-1111-1111-111111111111"), "Overhead pressing for shoulders.", null, "Shoulder Press" }
                });

            migrationBuilder.InsertData(
                table: "WorkoutExercises",
                columns: new[] { "Id", "ExerciseId", "Notes", "Order", "WorkoutId" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000002"), new Guid("aaaaaaa1-1111-1111-1111-111111111111"), null, 1, new Guid("90000000-0000-0000-0000-000000000001") },
                    { new Guid("90000000-0000-0000-0000-000000000003"), new Guid("aaaaaaa5-1111-1111-1111-111111111111"), null, 2, new Guid("90000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "WorkoutSets",
                columns: new[] { "Id", "DurationSeconds", "Reps", "SetNumber", "Weight", "WorkoutExerciseId" },
                values: new object[,]
                {
                    { new Guid("90000000-0000-0000-0000-000000000004"), null, 10, 1, 80m, new Guid("90000000-0000-0000-0000-000000000002") },
                    { new Guid("90000000-0000-0000-0000-000000000005"), null, 8, 2, 85m, new Guid("90000000-0000-0000-0000-000000000002") },
                    { new Guid("90000000-0000-0000-0000-000000000006"), null, 12, 1, 25m, new Guid("90000000-0000-0000-0000-000000000003") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_BodyGroupId",
                table: "Exercises",
                column: "BodyGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutId",
                table: "WorkoutExercises",
                column: "WorkoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSets_WorkoutExerciseId",
                table: "WorkoutSets",
                column: "WorkoutExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutSets");

            migrationBuilder.DropTable(
                name: "WorkoutExercises");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Workouts");

            migrationBuilder.DropTable(
                name: "BodyGroups");
        }
    }
}
