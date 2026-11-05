using System.ComponentModel.DataAnnotations;

namespace ProjektAWiMDDD.Models
{
    public class TrainingPlan
    {
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string UserId { get; set; } = "";
        public User User { get; set; } = null!;
        public List<PlanExercise> Exercises { get; set; } = new();
    }
}
