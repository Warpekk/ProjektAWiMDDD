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
    public async Task<WorkoutSessionStartResponse?> StartWorkoutSessionAsync(
    WorkoutSessionRequest request)
    {
        var response = await http.PostAsJsonAsync("api/workoutsessions", request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<WorkoutSessionStartResponse>();
    }

    public async Task<bool> AddWorkoutSetAsync(int sessionId, WorkoutSetRequest request)
    {
        var response = await http.PostAsJsonAsync(
            $"api/workoutsessions/{sessionId}/sets", request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> FinishWorkoutSessionAsync(int sessionId)
    {
        var response = await http.PutAsync(
            $"api/workoutsessions/{sessionId}/finish", null);

        return response.IsSuccessStatusCode;
    }
    public async Task<List<WorkoutSessionDto>> GetWorkoutSessionsAsync()
    {
        return await http.GetFromJsonAsync<List<WorkoutSessionDto>>(
            "api/workoutsessions") ?? [];
    }

    public async Task<WorkoutSessionDetailsDto?> GetWorkoutSessionByIdAsync(int id)
    {
        return await http.GetFromJsonAsync<WorkoutSessionDetailsDto>(
            $"api/workoutsessions/{id}");
    }

    public async Task<List<WorkoutSessionExerciseDto>> GetSessionExercisesAsync(int sessionId)
    {
        return await http.GetFromJsonAsync<List<WorkoutSessionExerciseDto>>(
            $"api/workoutsessions/{sessionId}/exercises") ?? [];
    }

    public async Task<bool> ToggleSessionExerciseAsync(int sessionId, int sessionExerciseId)
    {
        var response = await http.PutAsync(
            $"api/workoutsessions/{sessionId}/exercises/{sessionExerciseId}/complete",
            null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddSessionExerciseAsync(
        int sessionId,
        AddSessionExerciseRequest request)
    {
        var response = await http.PostAsJsonAsync(
            $"api/workoutsessions/{sessionId}/exercises",
            request);

        return response.IsSuccessStatusCode;
    }

    public async Task<StatisticsDto?> GetStatisticsAsync()
    {
        return await http.GetFromJsonAsync<StatisticsDto>("api/statistics");
    }
}