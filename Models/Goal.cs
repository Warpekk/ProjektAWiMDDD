using ProjektAWiMDDD.Models.Enums;

namespace ProjektAWiMDDD.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public User User { get; set; } = null!;
        public GoalType Type { get; set; }
        public int? ExerciseId { get; set; }
        public Exercise? Exercise { get; set; }
        public string Description { get; set; } = "";
        public decimal TargetValue { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsAchieved { get; set; }
    }
}
