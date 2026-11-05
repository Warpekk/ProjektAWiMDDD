namespace ProjektAWiMDDD.Models
{
    public class WorkoutSession
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public User User { get; set; } = null!;
        public int? TrainingPlanId { get; set; }
        public TrainingPlan? TrainingPlan { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int? DurationMinutes { get; set; }
        public string? Notes { get; set; }
        public List<WorkoutSet> Sets { get; set; } = new();
    }
}
