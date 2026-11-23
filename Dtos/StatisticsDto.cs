namespace ProjektAWiMDDD.Dtos
{
    public record StatisticsDto(
        int TotalWorkouts,
        int TotalDurationMinutes,
        int TotalSets,
        decimal TotalVolume,
        decimal AverageVolumePerWorkout,
        decimal AverageDurationMinutes,
        int CurrentMonthWorkouts,
        int PreviousMonthWorkouts,
        List<MonthlyStatisticsDto> MonthlyStatistics,
        List<MuscleGroupStatisticsDto> MuscleGroupStatistics,
        List<TopExerciseStatisticsDto> TopExercises,
        List<ExerciseRecordDto> ExerciseRecords
    );

    public record MonthlyStatisticsDto(
        string Month,
        int Workouts,
        decimal Volume
    );

    public record MuscleGroupStatisticsDto(
        string MuscleGroup,
        int Sets,
        decimal Volume
    );

    public record TopExerciseStatisticsDto(
        string ExerciseName,
        int Sets,
        decimal Volume
    );

    public record ExerciseRecordDto(
        string ExerciseName,
        decimal MaxWeight
    );
}
