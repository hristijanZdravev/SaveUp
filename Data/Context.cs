using Microsoft.EntityFrameworkCore;
using SaveUp.Models;

namespace SaveUp.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) :
            base(options)
        {
        }

        public DbSet<BodyGroup> BodyGroups { get; set; }
        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<WorkoutSet> WorkoutSets { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureEntinties();
            modelBuilder.SeedData().Wait();
        }

    }
}
