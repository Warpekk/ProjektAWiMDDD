using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektAWiMDDD.Data;
using ProjektAWiMDDD.Dtos;
using ProjektAWiMDDD.Models.Enums;

namespace ProjektAWiMDDD.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class ExercisesController(AppDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<ExerciseDto>> Get([FromQuery] string? search, [FromQuery] MuscleGroup? group)
        {
            var q = db.Exercises.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search)) 
                q = q.Where(e => e.Name.Contains(search));

            if (group is not null) 
                q = q.Where(e => e.MuscleGroup == group);

            return await q.OrderBy(e => e.Name)
                .Select(e => new ExerciseDto(e.Id, e.Name, e.Description, e.MuscleGroup.ToString(), e.Equipment))
                .ToListAsync();
        }
    }
}
