namespace ProjektAWiMDDD.Models
{
    public class WorkoutSessionExercise
    {
        public int Id { get; set; }

        public int WorkoutSessionId { get; set; }
        public WorkoutSession WorkoutSession { get; set; } = null!;

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;

        public int Order { get; set; }
        public int PlannedSets { get; set; }
        public int PlannedReps { get; set; }
        public decimal? PlannedWeight { get; set; }

        public bool IsCompleted { get; set; } = false;

        public bool IsAdditional { get; set; } = false;
    }
}