using ProjektAWiMDDD.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjektAWiMDDD.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = "";
        [MaxLength(1000)] public string? Description { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
        public string? Equipment { get; set; }
    }
}
