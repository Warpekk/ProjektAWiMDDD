using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektAWiMDDD.Data;
using ProjektAWiMDDD.Dtos;
using ProjektAWiMDDD.Models;
using ProjektAWiMDDD.Models.Enums;

namespace ProjektAWiMDDD.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExercisesController(AppDbContext db) : ControllerBase
    {
        // GET
        [HttpGet]
        public async Task<IEnumerable<ExerciseDto>> Get(
            [FromQuery] string? search,
            [FromQuery] MuscleGroup? group)
        {
            var q = db.Exercises.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(e => e.Name.Contains(search));

            if (group is not null)
                q = q.Where(e => e.MuscleGroup == group);

            return await q.OrderBy(e => e.Name)
                .Select(e => new ExerciseDto(
                    e.Id,
                    e.Name,
                    e.Description,
                    e.MuscleGroup.ToString(),
                    e.Equipment))
                .ToListAsync();
        }

        // GET/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exercise = await db.Exercises
                .Where(e => e.Id == id)
                .Select(e => new ExerciseDto(
                    e.Id,
                    e.Name,
                    e.Description,
                    e.MuscleGroup.ToString(),
                    e.Equipment))
                .FirstOrDefaultAsync();

            if (exercise == null)
                return NotFound();

            return Ok(exercise);
        }

        // POST 
        [HttpPost]
        public async Task<IActionResult> Create(ExerciseRequest request)
        {
            var exercise = new Exercise
            {
                Name = request.Name,
                Description = request.Description,
                MuscleGroup = request.MuscleGroup,
                Equipment = request.Equipment
            };

            db.Exercises.Add(exercise);
            await db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = exercise.Id },
                new ExerciseDto(
                    exercise.Id,
                    exercise.Name,
                    exercise.Description,
                    exercise.MuscleGroup.ToString(),
                    exercise.Equipment));
        }

        // PUT/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ExerciseRequest request)
        {
            var exercise = await db.Exercises.FindAsync(id);

            if (exercise == null)
                return NotFound();

            exercise.Name = request.Name;
            exercise.Description = request.Description;
            exercise.MuscleGroup = request.MuscleGroup;
            exercise.Equipment = request.Equipment;

            await db.SaveChangesAsync();

            return Ok(new
            {
                message = "Ćwiczenie zostało zaktualizowane.",
                exercise.Id,
                exercise.Name
            });
        }

        // DELETE/{id} 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exercise = await db.Exercises.FindAsync(id);

            if (exercise == null)
                return NotFound();

            var isUsed = await db.Entry(exercise)
                .Collection(e => e.PlanExercises)
                .Query()
                .AnyAsync();

            if (isUsed)
                return Conflict("Nie można usunąć ćwiczenia, ponieważ jest przypisane do planu treningowego.");

            db.Exercises.Remove(exercise);
            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}
