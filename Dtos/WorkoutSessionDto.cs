using System.ComponentModel.DataAnnotations;

namespace ProjektAWiMDDD.Dtos
{
    public class WorkoutSessionRequest
    {
        public int? TrainingPlanId { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class WorkoutSetRequest
    {
        [Range(1, int.MaxValue)]
        public int ExerciseId { get; set; }

        [Range(1, 100)]
        public int SetNumber { get; set; }

        [Range(1, 1000)]
        public int Reps { get; set; }

        [Range(typeof(decimal), "0", "1000")]
        public decimal Weight { get; set; }
    }

    public record WorkoutSessionDto(
        int Id,
        DateTime Date,
        int? DurationMinutes,
        string? Notes,
        int? TrainingPlanId,
        string? TrainingPlanName,
        int SetCount
    );

    public record WorkoutSessionDetailsDto(
        int Id,
        DateTime Date,
        int? DurationMinutes,
        string? Notes,
        string? TrainingPlanName,
        List<WorkoutSetDto> Sets
    );
    public record WorkoutSetDto(
        int Id,
        int ExerciseId,
        string ExerciseName,
        int SetNumber,
        int Reps,
        decimal Weight
    );
}
