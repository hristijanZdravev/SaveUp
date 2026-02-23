using Microsoft.EntityFrameworkCore;
using PeakLift.Models;

namespace PeakLift.Data
{
    public static class ModelBuilderExtensions
    {

        public static void ConfigureEntinties(this ModelBuilder builder)
        {

        }

        public static class DbInitializer
        {
            public static async Task SeedAsync(Context context)
            {
                // seed only once
                if (await context.Workouts.AnyAsync())
                    return;

                var random = new Random();

                // =========================
                // BODY GROUPS
                // =========================
                var chest = new BodyGroup { Id = Guid.NewGuid(), Name = "Chest" };
                var back = new BodyGroup { Id = Guid.NewGuid(), Name = "Back" };
                var legs = new BodyGroup { Id = Guid.NewGuid(), Name = "Legs" };
                var shoulders = new BodyGroup { Id = Guid.NewGuid(), Name = "Shoulders" };
                var arms = new BodyGroup { Id = Guid.NewGuid(), Name = "Arms" };

                context.BodyGroups.AddRange(chest, back, legs, shoulders, arms);

                // =========================
                // EXERCISES (25)
                // =========================
                var exercises = new List<Exercise>
                {
                    new() { Id=Guid.NewGuid(), Title="Bench Press", Description="Chest press", BodyGroupId=chest.Id, ImagePublicId="bench_press" },
                    new() { Id=Guid.NewGuid(), Title="Incline Dumbbell Press", Description="Upper chest", BodyGroupId=chest.Id, ImagePublicId="incline_press" },
                    new() { Id=Guid.NewGuid(), Title="Pull Ups", Description="Back", BodyGroupId=back.Id, ImagePublicId="pull_ups" },
                    new() { Id=Guid.NewGuid(), Title="Lat Pulldown", Description="Back", BodyGroupId=back.Id, ImagePublicId="lat_pulldown" },
                    new() { Id=Guid.NewGuid(), Title="Squat", Description="Legs", BodyGroupId=legs.Id, ImagePublicId="squat" },
                    new() { Id=Guid.NewGuid(), Title="Leg Press", Description="Legs", BodyGroupId=legs.Id, ImagePublicId="leg_press" },
                    new() { Id=Guid.NewGuid(), Title="Shoulder Press", Description="Shoulders", BodyGroupId=shoulders.Id, ImagePublicId="shoulder_press" },
                    new() { Id=Guid.NewGuid(), Title="Lateral Raise", Description="Shoulders", BodyGroupId=shoulders.Id, ImagePublicId="lateral_raise" },
                    new() { Id=Guid.NewGuid(), Title="Barbell Curl", Description="Biceps", BodyGroupId=arms.Id, ImagePublicId="barbell_curl" },
                    new() { Id=Guid.NewGuid(), Title="Tricep Pushdown", Description="Triceps", BodyGroupId=arms.Id, ImagePublicId="tricep_pushdown" }
                };

                context.Exercises.AddRange(exercises);

                // =========================
                // WORKOUTS (22)
                // =========================
                var userId = "user@test.com";

                for (int i = 1; i <= 22; i++)
                {
                    var workout = new Workout
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Title = $"Workout {i}",
                        Date = DateTime.UtcNow.AddDays(-i * 2)
                    };

                    context.Workouts.Add(workout);

                    var selectedExercises = exercises
                        .OrderBy(x => random.Next())
                        .Take(3)
                        .ToList();

                    int order = 1;

                    foreach (var ex in selectedExercises)
                    {
                        var workoutExercise = new WorkoutExercise
                        {
                            Id = Guid.NewGuid(),
                            WorkoutId = workout.Id,
                            ExerciseId = ex.Id,
                            Order = order++
                        };

                        context.WorkoutExercises.Add(workoutExercise);

                        for (int s = 1; s <= 3; s++)
                        {
                            context.WorkoutSets.Add(new WorkoutSet
                            {
                                Id = Guid.NewGuid(),
                                WorkoutExerciseId = workoutExercise.Id,
                                SetNumber = s,
                                Reps = random.Next(6, 13),
                                Weight = random.Next(20, 100)
                            });
                        }
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
