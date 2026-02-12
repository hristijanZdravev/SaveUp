using Microsoft.EntityFrameworkCore;
using SaveUp.Models;

namespace SaveUp.Data
{
    public static class ModelBuilderExtensions
    {

        public static void ConfigureEntinties(this ModelBuilder builder)
        {

        }

        public static Task SeedData(this ModelBuilder modelBuilder)
        {
            // =========================
            // BODY GROUPS
            // =========================
            var chestId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var backId = Guid.Parse("22222222-1111-1111-1111-111111111111");
            var legsId = Guid.Parse("33333333-1111-1111-1111-111111111111");
            var shouldersId = Guid.Parse("44444444-1111-1111-1111-111111111111");

            BodyGroup[] bodyGroups =
            [
                new BodyGroup { Id = chestId, Name = "Chest", SubGroup = null },
        new BodyGroup { Id = backId, Name = "Back", SubGroup = null },
        new BodyGroup { Id = legsId, Name = "Legs", SubGroup = null },
        new BodyGroup { Id = shouldersId, Name = "Shoulders", SubGroup = null }
            ];

            modelBuilder.Entity<BodyGroup>().HasData(bodyGroups);

            // =========================
            // EXERCISES
            // =========================
            var benchId = Guid.Parse("aaaaaaa1-1111-1111-1111-111111111111");
            var inclineId = Guid.Parse("aaaaaaa2-1111-1111-1111-111111111111");
            var pullupId = Guid.Parse("aaaaaaa3-1111-1111-1111-111111111111");
            var squatId = Guid.Parse("aaaaaaa4-1111-1111-1111-111111111111");
            var shoulderPressId = Guid.Parse("aaaaaaa5-1111-1111-1111-111111111111");

            Exercise[] exercises =
            [
                new Exercise
        {
            Id = benchId,
            Title = "Barbell Bench Press",
            Description = "Compound chest pressing movement.",
            BodyGroupId = chestId
        },
        new Exercise
        {
            Id = inclineId,
            Title = "Incline Dumbbell Press",
            Description = "Upper chest focused pressing movement.",
            BodyGroupId = chestId
        },
        new Exercise
        {
            Id = pullupId,
            Title = "Pull Ups",
            Description = "Bodyweight vertical pulling movement.",
            BodyGroupId = backId
        },
        new Exercise
        {
            Id = squatId,
            Title = "Barbell Squats",
            Description = "Compound lower body movement.",
            BodyGroupId = legsId
        },
        new Exercise
        {
            Id = shoulderPressId,
            Title = "Shoulder Press",
            Description = "Overhead pressing for shoulders.",
            BodyGroupId = shouldersId
        }
            ];

            modelBuilder.Entity<Exercise>().HasData(exercises);

            // =========================
            // WORKOUT
            // =========================
            var workoutId = Guid.Parse("90000000-0000-0000-0000-000000000001");

            Workout[] workouts =
            [
                new Workout
        {
            Id = workoutId,
            UserId = "user@test.com",
            Date = new DateTime(2026, 2, 10),
            Title = "Push Day"
        }
            ];

            modelBuilder.Entity<Workout>().HasData(workouts);

            // =========================
            // WORKOUT EXERCISES
            // =========================
            var workoutBenchId = Guid.Parse("90000000-0000-0000-0000-000000000002");
            var workoutShoulderId = Guid.Parse("90000000-0000-0000-0000-000000000003");

            WorkoutExercise[] workoutExercises =
            [
                new WorkoutExercise
        {
            Id = workoutBenchId,
            WorkoutId = workoutId,
            ExerciseId = benchId,
            Order = 1,
            Notes = null
        },
        new WorkoutExercise
        {
            Id = workoutShoulderId,
            WorkoutId = workoutId,
            ExerciseId = shoulderPressId,
            Order = 2,
            Notes = null
        }
            ];

            modelBuilder.Entity<WorkoutExercise>().HasData(workoutExercises);

            // =========================
            // WORKOUT SETS
            // =========================
            WorkoutSet[] workoutSets =
            [
                new WorkoutSet
        {
            Id = Guid.Parse("90000000-0000-0000-0000-000000000004"),
            WorkoutExerciseId = workoutBenchId,
            SetNumber = 1,
            Reps = 10,
            Weight = 80,
            DurationSeconds = null
        },
        new WorkoutSet
        {
            Id = Guid.Parse("90000000-0000-0000-0000-000000000005"),
            WorkoutExerciseId = workoutBenchId,
            SetNumber = 2,
            Reps = 8,
            Weight = 85,
            DurationSeconds = null
        },
        new WorkoutSet
        {
            Id = Guid.Parse("90000000-0000-0000-0000-000000000006"),
            WorkoutExerciseId = workoutShoulderId,
            SetNumber = 1,
            Reps = 12,
            Weight = 25,
            DurationSeconds = null
        }
            ];

            modelBuilder.Entity<WorkoutSet>().HasData(workoutSets);

            return Task.CompletedTask;
        }
    }
}   
