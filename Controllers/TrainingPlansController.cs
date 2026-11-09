using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ProjektAWiMDDD.Data;
using ProjektAWiMDDD.Models;
using ProjektAWiMDDD.Dtos;

namespace ProjektAWiMDDD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TrainingPlansController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TrainingPlansController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/trainingplans
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var plans = await _db.TrainingPlans
                .Where(p => p.UserId == userId)
                .Select(p => new TrainingPlanDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Exercises.Count
                ))
                .ToListAsync();

            return Ok(plans);
        }

        // GET: api/trainingplans/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var plan = await _db.TrainingPlans
                .Where(p => p.Id == id && p.UserId == userId)
                .Select(p => new TrainingPlanDetailsDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Exercises.Select(e => new PlanExerciseDetailsDto(
                        e.ExerciseId,
                        e.Exercise.Name,
                        e.Exercise.MuscleGroup.ToString(),
                        e.Order,
                        e.PlannedSets,
                        e.PlannedReps,
                        e.PlannedWeight
                    )).ToList()
                ))
                .FirstOrDefaultAsync();

            if (plan == null)
                return NotFound();

            return Ok(plan);
        }

        // POST: api/trainingplans
        [HttpPost]
        public async Task<IActionResult> Create(TrainingPlanRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var plan = new TrainingPlan
            {
                Name = request.Name,
                Description = request.Description,
                UserId = userId,
                Exercises = request.Exercises.Select(e => new PlanExercise
                {
                    ExerciseId = e.ExerciseId,
                    Order = e.Order,
                    PlannedSets = e.PlannedSets,
                    PlannedReps = e.PlannedReps,
                    PlannedWeight = e.PlannedWeight
                }).ToList()
            };

            _db.TrainingPlans.Add(plan);
            await _db.SaveChangesAsync();

            return Ok(new { plan.Id, plan.Name });
        }


        // PUT: api/trainingplans/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TrainingPlanRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var plan = await _db.TrainingPlans
                .Include(p => p.Exercises)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (plan == null)
                return NotFound();

            var exerciseIds = request.Exercises
                .Select(e => e.ExerciseId)
                .Distinct()
                .ToList();

            var existingExercisesCount = await _db.Exercises
                .CountAsync(e => exerciseIds.Contains(e.Id));

            if (existingExercisesCount != exerciseIds.Count)
                return BadRequest("Jedno lub więcej ćwiczeń nie istnieje.");
            plan.Name = request.Name;
            plan.Description = request.Description;
            plan.Exercises.Clear();
            foreach (var exercise in request.Exercises)
            {
                plan.Exercises.Add(new PlanExercise
                {
                    ExerciseId = exercise.ExerciseId,
                    Order = exercise.Order,
                    PlannedSets = exercise.PlannedSets,
                    PlannedReps = exercise.PlannedReps,
                    PlannedWeight = exercise.PlannedWeight
                });
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Plan treningowy został zaktualizowany.",
                plan.Id,
                plan.Name
            });
        }
        


        // DELETE: api/trainingplans/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var plan = await _db.TrainingPlans
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (plan == null)
                return NotFound();

            _db.TrainingPlans.Remove(plan);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}