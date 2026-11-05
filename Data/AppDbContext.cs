using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektAWiMDDD.Models;
using ProjektAWiMDDD.Models.Enums;

namespace ProjektAWiMDDD.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<TrainingPlan> TrainingPlans => Set<TrainingPlan>();
        public DbSet<PlanExercise> PlanExercises => Set<PlanExercise>();
        public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
        public DbSet<WorkoutSet> WorkoutSets => Set<WorkoutSet>();
        public DbSet<Goal> Goals => Set<Goal>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<WorkoutSet>().Property(x => x.Weight).HasPrecision(7, 2);
            b.Entity<PlanExercise>().Property(x => x.PlannedWeight).HasPrecision(7, 2);
            b.Entity<Goal>().Property(x => x.TargetValue).HasPrecision(8, 2);

            b.Entity<WorkoutSession>().HasOne(w => w.TrainingPlan).WithMany().OnDelete(DeleteBehavior.Restrict);
            b.Entity<WorkoutSet>().HasOne(s => s.Exercise).WithMany().OnDelete(DeleteBehavior.Restrict);
            b.Entity<PlanExercise>().HasOne(p => p.Exercise).WithMany().OnDelete(DeleteBehavior.Restrict);
            b.Entity<Goal>().HasOne(g => g.Exercise).WithMany().OnDelete(DeleteBehavior.Restrict);

            //Przykładowe dane na start
            b.Entity<Exercise>().HasData(
                new Exercise { Id = 1, Name = "Wyciskanie sztangi leżąc", MuscleGroup = MuscleGroup.Chest, Equipment = "Sztanga" },
                new Exercise { Id = 2, Name = "Przysiad ze sztangą", MuscleGroup = MuscleGroup.Legs, Equipment = "Sztanga" },
                new Exercise { Id = 3, Name = "Martwy ciąg", MuscleGroup = MuscleGroup.Back, Equipment = "Sztanga" },
                new Exercise { Id = 4, Name = "Podciąganie na drążku", MuscleGroup = MuscleGroup.Back, Equipment = "Drążek" },
                new Exercise { Id = 5, Name = "Plank", MuscleGroup = MuscleGroup.Core }
            );
        }
    }
}
