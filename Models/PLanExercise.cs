namespace ProjektAWiMDDD.Models
{
    public class PlanExercise
    {
        public int Id { get; set; }
        public int TrainingPlanId { get; set; }
        public TrainingPlan TrainingPlan { get; set; } = null!;
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;
        public int Order { get; set; }
        public int PlannedSets { get; set; }
        public int PlannedReps { get; set; }
        public decimal? PlannedWeight { get; set; }
    }
}
