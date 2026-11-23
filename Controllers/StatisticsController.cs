using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektAWiMDDD.Data;
using ProjektAWiMDDD.Dtos;

namespace ProjektAWiMDDD.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController(AppDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<StatisticsDto>> GetStatistics()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var sessions = await db.WorkoutSessions
                .AsNoTracking()
                .Where(s => s.UserId == userId &&
                            s.DurationMinutes != null)
                .Select(s => new
                {
                    s.Id,
                    s.Date,
                    Duration = s.DurationMinutes!.Value
                })
                .ToListAsync();

            var sessionIds = sessions.Select(s => s.Id).ToList();

            var sets = await db.WorkoutSets
                .AsNoTracking()
                .Where(s => sessionIds.Contains(s.WorkoutSessionId))
                .Select(s => new
                {
                    s.WorkoutSessionId,
                    s.ExerciseId,
                    ExerciseName = s.Exercise.Name,
                    MuscleGroup = s.Exercise.MuscleGroup.ToString(),
                    s.Reps,
                    s.Weight,
                    Volume = s.Weight * s.Reps
                })
                .ToListAsync();

            var totalWorkouts = sessions.Count;
            var totalDuration = sessions.Sum(s => s.Duration);
            var totalSets = sets.Count;
            var totalVolume = sets.Sum(s => s.Volume);

            var now = DateTime.UtcNow;
            var currentMonth = new DateTime(now.Year, now.Month, 1);
            var previousMonth = currentMonth.AddMonths(-1);

            var currentMonthWorkouts = sessions.Count(s =>
                s.Date >= currentMonth &&
                s.Date < currentMonth.AddMonths(1));

            var previousMonthWorkouts = sessions.Count(s =>
                s.Date >= previousMonth &&
                s.Date < currentMonth);

            var monthlyStatistics = sessions
                .GroupBy(s => new
                {
                    s.Date.Year,
                    s.Date.Month
                })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g =>
                {
                    var monthSessionIds = g.Select(s => s.Id).ToHashSet();

                    var monthVolume = sets
                        .Where(s => monthSessionIds.Contains(s.WorkoutSessionId))
                        .Sum(s => s.Volume);

                    return new MonthlyStatisticsDto(
                        $"{g.Key.Month:D2}/{g.Key.Year}",
                        g.Count(),
                        monthVolume
                    );
                })
                .ToList();

            var muscleGroupStatistics = sets
                .GroupBy(s => s.MuscleGroup)
                .Select(g => new MuscleGroupStatisticsDto(
                    g.Key,
                    g.Count(),
                    g.Sum(s => s.Volume)
                ))
                .OrderByDescending(g => g.Volume)
                .ToList();

            var topExercises = sets
                .GroupBy(s => s.ExerciseName)
                .Select(g => new TopExerciseStatisticsDto(
                    g.Key,
                    g.Count(),
                    g.Sum(s => s.Volume)
                ))
                .OrderByDescending(g => g.Sets)
                .Take(5)
                .ToList();

            var exerciseRecords = sets
                .GroupBy(s => s.ExerciseName)
                .Select(g => new ExerciseRecordDto(
                    g.Key,
                    g.Max(s => s.Weight)
                ))
                .OrderByDescending(g => g.MaxWeight)
                .Take(10)
                .ToList();

            var result = new StatisticsDto(
                totalWorkouts,
                totalDuration,
                totalSets,
                totalVolume,
                totalWorkouts > 0 ? totalVolume / totalWorkouts : 0,
                totalWorkouts > 0 ? (decimal)totalDuration / totalWorkouts : 0,
                currentMonthWorkouts,
                previousMonthWorkouts,
                monthlyStatistics,
                muscleGroupStatistics,
                topExercises,
                exerciseRecords
            );

            return Ok(result);
        }
    }
}
