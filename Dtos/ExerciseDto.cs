using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ProjektAWiMDDD.Models.Enums;

namespace ProjektAWiMDDD.Dtos
{
    public record ExerciseDto(
        int Id,
        string Name,
        string? Description,
        string MuscleGroup,
        string? Equipment
    );

    public class ExerciseRequest
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(1000)]
        public string? Description { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MuscleGroup MuscleGroup { get; set; }

        public string? Equipment { get; set; }
    }
}