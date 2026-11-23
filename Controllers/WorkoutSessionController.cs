
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektAWiMDDD.Data;
using ProjektAWiMDDD.Dtos;
using ProjektAWiMDDD.Models;

namespace ProjektAWiMDDD.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutSessionsController(AppDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var sessions = await db.WorkoutSessions
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .Select(w => new WorkoutSessionDto(
                    w.Id,
                    w.Date,
                    w.DurationMinutes,
                    w.Notes,
                    w.TrainingPlanId,
                    w.TrainingPlan != null ? w.TrainingPlan.Name : null,
                    w.Sets.Count
                ))
                .ToListAsync();

            return Ok(sessions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var session = await db.WorkoutSessions
                .Where(w => w.Id == id && w.UserId == userId)
                .Select(w => new WorkoutSessionDetailsDto(
                    w.Id,
                    w.Date,
                    w.DurationMinutes,
                    w.Notes,
                    w.TrainingPlan != null ? w.TrainingPlan.Name : null,
                    w.Sets.OrderBy(s => s.SetNumber)
                        .Select(s => new WorkoutSetDto(
                            s.Id,
                            s.ExerciseId,
                            s.Exercise.Name,
                            s.SetNumber,
                            s.Reps,
                            s.Weight
                        )).ToList()
                ))
                .FirstOrDefaultAsync();

            if (session == null)
                return NotFound();

            return Ok(session);
        }

        [HttpGet("{id}/exercises")]
        public async Task<IActionResult> GetSessionExercises(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var sessionExists = await db.WorkoutSessions
                .AnyAsync(w => w.Id == id && w.UserId == userId);

            if (!sessionExists)
                return NotFound("Nie znaleziono treningu.");

            var exercises = await db.WorkoutSessionExercises
                .Where(e => e.WorkoutSessionId == id)
                .OrderBy(e => e.Order)
                .Select(e => new WorkoutSessionExerciseDto(
                    e.Id,
                    e.ExerciseId,
                    e.Exercise.Name,
                    e.Order,
                    e.PlannedSets,
                    e.PlannedReps,
                    e.PlannedWeight,
                    e.IsCompleted,
                    e.IsAdditional
                ))
                .ToListAsync();

            return Ok(exercises);
        }

        [HttpPut("{id}/exercises/{exerciseId}/complete")]
        public async Task<IActionResult> CompleteExercise(int id, int exerciseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var exercise = await db.WorkoutSessionExercises
                .FirstOrDefaultAsync(e =>
                    e.Id == exerciseId &&
                    e.WorkoutSessionId == id &&
                    e.WorkoutSession.UserId == userId);

            if (exercise == null)
                return NotFound("Nie znaleziono ćwiczenia w tym treningu.");

            if (await db.WorkoutSessions.AnyAsync(w =>
                w.Id == id && w.DurationMinutes != null))
                return BadRequest("Trening został już zakończony.");

            exercise.IsCompleted = !exercise.IsCompleted;

            await db.SaveChangesAsync();

            return Ok(new
            {
                exercise.Id,
                exercise.IsCompleted,
                message = "Zmieniono status ćwiczenia."
            });
        }

        [HttpPost("{id}/exercises")]
        public async Task<IActionResult> AddSessionExercise(
    int id,
    AddSessionExerciseRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var session = await db.WorkoutSessions
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (session == null)
                return NotFound("Nie znaleziono treningu.");

            if (session.DurationMinutes.HasValue)
                return BadRequest("Trening został już zakończony.");

            var exerciseExists = await db.Exercises
                .AnyAsync(e => e.Id == request.ExerciseId);

            if (!exerciseExists)
                return BadRequest("Wybrane ćwiczenie nie istnieje.");

            var alreadyAdded = await db.WorkoutSessionExercises
                .AnyAsync(e =>
                    e.WorkoutSessionId == id &&
                    e.ExerciseId == request.ExerciseId);

            if (alreadyAdded)
                return BadRequest("To ćwiczenie jest już przypisane do treningu.");

            var maxOrder = await db.WorkoutSessionExercises
                .Where(e => e.WorkoutSessionId == id)
                .Select(e => (int?)e.Order)
                .MaxAsync() ?? 0;

            var sessionExercise = new WorkoutSessionExercise
            {
                WorkoutSessionId = id,
                ExerciseId = request.ExerciseId,
                Order = maxOrder + 1,
                PlannedSets = 0,
                PlannedReps = 0,
                PlannedWeight = null,
                IsCompleted = false,
                IsAdditional = true
            };

            db.WorkoutSessionExercises.Add(sessionExercise);
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Dodano dodatkowe ćwiczenie.",
                sessionExercise.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Start(WorkoutSessionRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<PlanExercise> planExercises = [];

            if (request.TrainingPlanId.HasValue)
            {
                var plan = await db.TrainingPlans
                    .FirstOrDefaultAsync(p =>
                        p.Id == request.TrainingPlanId.Value &&
                        p.UserId == userId);

                if (plan == null)
                    return BadRequest("Wybrany plan treningowy nie istnieje.");

                planExercises = await db.PlanExercises
                    .Where(p => p.TrainingPlanId == plan.Id)
                    .ToListAsync();
            }

            var session = new WorkoutSession
            {
                UserId = userId!,
                TrainingPlanId = request.TrainingPlanId,
                Notes = request.Notes,
                Date = DateTime.UtcNow
            };

            db.WorkoutSessions.Add(session);
            await db.SaveChangesAsync();

            var sessionExercises = planExercises.Select(p =>
                new WorkoutSessionExercise
                {
                    WorkoutSessionId = session.Id,
                    ExerciseId = p.ExerciseId,
                    Order = p.Order,
                    PlannedSets = p.PlannedSets,
                    PlannedReps = p.PlannedReps,
                    PlannedWeight = p.PlannedWeight,
                    IsCompleted = false,
                    IsAdditional = false
                }).ToList();

            if (sessionExercises.Count > 0)
            {
                db.WorkoutSessionExercises.AddRange(sessionExercises);
                await db.SaveChangesAsync();
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = session.Id },
                new
                {
                    session.Id,
                    session.Date,
                    message = "Trening został rozpoczęty."
                }
            );
        }

        [HttpPost("{id}/sets")]
        public async Task<IActionResult> AddSet(int id, WorkoutSetRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var session = await db.WorkoutSessions
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (session == null)
                return NotFound("Nie znaleziono treningu.");

            if (session.DurationMinutes.HasValue)
                return BadRequest("Nie można dodawać serii do zakończonego treningu.");

            var exerciseExists = await db.Exercises
                .AnyAsync(e => e.Id == request.ExerciseId);

            if (!exerciseExists)
                return BadRequest("Wybrane ćwiczenie nie istnieje.");

            var set = new WorkoutSet
            {
                WorkoutSessionId = session.Id,
                ExerciseId = request.ExerciseId,
                SetNumber = request.SetNumber,
                Reps = request.Reps,
                Weight = request.Weight
            };

            db.WorkoutSets.Add(set);
            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Seria została dodana.",
                set.Id,
                set.ExerciseId,
                set.SetNumber,
                set.Reps,
                set.Weight
            });
        }

        [HttpPut("{id}/finish")]
        public async Task<IActionResult> Finish(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var session = await db.WorkoutSessions
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (session == null)
                return NotFound("Nie znaleziono treningu.");

            if (session.DurationMinutes.HasValue)
                return BadRequest("Ten trening został już zakończony.");

            var duration = (int)Math.Ceiling(
                (DateTime.UtcNow - session.Date).TotalMinutes
            );

            session.DurationMinutes = Math.Max(0, duration);

            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Trening został zakończony.",
                session.Id,
                session.DurationMinutes
            });
        }
    }
}

