
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

        [HttpPost]
        public async Task<IActionResult> Start(WorkoutSessionRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (request.TrainingPlanId.HasValue)
            {
                var planExists = await db.TrainingPlans.AnyAsync(p =>
                    p.Id == request.TrainingPlanId.Value &&
                    p.UserId == userId);

                if (!planExists)
                    return BadRequest("Wybrany plan treningowy nie istnieje.");
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

            return CreatedAtAction(
                nameof(GetById),
                new { id = session.Id },
                new { session.Id, session.Date, message = "Trening został rozpoczęty." }
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
