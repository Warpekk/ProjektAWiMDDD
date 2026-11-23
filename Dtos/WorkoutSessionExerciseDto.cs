namespace ProjektAWiMDDD.Dtos
{
    public record WorkoutSessionExerciseDto(
        int Id,
        int ExerciseId,
        string ExerciseName,
        int Order,
        int PlannedSets,
        int PlannedReps,
        decimal? PlannedWeight,
        bool IsCompleted,
        bool IsAdditional
    );

    public class AddSessionExerciseRequest
    {
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
        public int ExerciseId { get; set; }
    }
}
