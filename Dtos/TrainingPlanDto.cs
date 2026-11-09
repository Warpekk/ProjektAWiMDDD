using System.ComponentModel.DataAnnotations;

namespace ProjektAWiMDDD.Dtos
{
    public record TrainingPlanDto(int Id, string Name, string? Description, int ExerciseCount);
    public record TrainingPlanDetailsDto(
    int Id,
    string Name,
    string? Description,
    List<PlanExerciseDetailsDto> Exercises
);

    public record PlanExerciseDetailsDto(
        int ExerciseId,
        string ExerciseName,
        string MuscleGroup,
        int Order,
        int PlannedSets,
        int PlannedReps,
        decimal? PlannedWeight
    );

    public class TrainingPlanRequest
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        public string? Description { get; set; }

        public List<PlanExerciseRequest> Exercises { get; set; } = new();
    }

    public class PlanExerciseRequest
    {
        [Range(1, int.MaxValue)]
        public int ExerciseId { get; set; }

        [Range(0, int.MaxValue)]
        public int Order { get; set; }

        [Range(1, 100)]
        public int PlannedSets { get; set; }

        [Range(1, 1000)]
        public int PlannedReps { get; set; }

        [Range(typeof(decimal), "0", "10000")]
        public decimal? PlannedWeight { get; set; }
    }
}
