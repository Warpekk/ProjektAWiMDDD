using System.Net.Http.Json;
using ProjektAWiMDDD.Dtos;
using ProjektAWiMDDD.Models.Enums;

namespace ProjektAWiMDDD.Services;

public class FitnessApiService(HttpClient http)
{
    public async Task<List<ExerciseDto>> GetExercisesAsync(string? search = null, MuscleGroup? group = null)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");
        if (group.HasValue)
            queryParams.Add($"group={group.Value}");

        var url = "api/exercises" + (queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "");
        return await http.GetFromJsonAsync<List<ExerciseDto>>(url) ?? [];
    }

    public async Task<List<TrainingPlanDto>> GetTrainingPlansAsync()
    {
        return await http.GetFromJsonAsync<List<TrainingPlanDto>>("api/trainingplans") ?? [];
    }

    public async Task<TrainingPlanDetailsDto?> GetTrainingPlanByIdAsync(int id)
    {
        return await http.GetFromJsonAsync<TrainingPlanDetailsDto>($"api/trainingplans/{id}");
    }

    public async Task<bool> CreateTrainingPlanAsync(TrainingPlanRequest request)
    {
        var response = await http.PostAsJsonAsync("api/trainingplans", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateTrainingPlanAsync(int id, TrainingPlanRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/trainingplans/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTrainingPlanAsync(int id)
    {
        var response = await http.DeleteAsync($"api/trainingplans/{id}");
        return response.IsSuccessStatusCode;
    }
}