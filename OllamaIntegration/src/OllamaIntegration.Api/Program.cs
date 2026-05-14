using System.Text.Json;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using OllamaIntegration.Api.Models;
using OllamaIntegration.Api.Models.Requests;
using OllamaIntegration.Api.Models.Responses;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("ollama", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Ollama:BaseUrl"]!);
    client.Timeout = TimeSpan.FromMinutes(5);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/chat", async (
    ChatRequest request,
    IHttpClientFactory clientFactory,
    IConfiguration config,
    ILogger<Program> logger) =>
{
    // --- Validate ---
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, validateAllProperties: true))
    {
        var errors = validationResults.Select(v => v.ErrorMessage).ToList();
        return Results.BadRequest(new ChatResponse
        {
            Modality = ModalityType.Text,
            Success = false,
            Error = string.Join("; ", errors)
        });
    }

    var model = config["Ollama:Model"] ?? "llama3.2:3b";
    var client = clientFactory.CreateClient("ollama");

    var ollamaRequest = new
    {
        model,
        stream = false,
        messages = request.Messages.Select(m => new { role = m.Role, content = m.Content })
    };

    try
    {
        var httpResponse = await client.PostAsJsonAsync("/api/chat", ollamaRequest);

        if (!httpResponse.IsSuccessStatusCode)
        {
            var body = await httpResponse.Content.ReadAsStringAsync();
            logger.LogError("Ollama returned {StatusCode}: {Body}", (int)httpResponse.StatusCode, body);
            return Results.Json(
                new ChatResponse { Modality = ModalityType.Text, Success = false, Error = "The model service returned an error." },
                statusCode: 502);
        }

        var ollamaResponse = await httpResponse.Content.ReadFromJsonAsync<OllamaApiResponse>();

        if (ollamaResponse?.Message is null)
        {
            logger.LogError("Ollama response was empty or missing message field.");
            return Results.Json(
                new ChatResponse { Modality = ModalityType.Text, Success = false, Error = "Unexpected response from model service." },
                statusCode: 502);
        }

        return Results.Ok(new ChatResponse
        {
            Modality = ModalityType.Text,
            Success = true,
            Model = ollamaResponse.Model,
            Message = new ChatReplyMessage
            {
                Role = ollamaResponse.Message.Role,
                Content = ollamaResponse.Message.Content
            }
        });
    }
    catch (HttpRequestException ex)
    {
        logger.LogError(ex, "Could not reach Ollama service at {BaseUrl}", config["Ollama:BaseUrl"]);
        return Results.Json(
            new ChatResponse { Modality = ModalityType.Text, Success = false, Error = "Could not reach the model service. Is Ollama running?" },
            statusCode: 503);
    }
})
.WithName("Chat")
.WithOpenApi();

app.Run();

// Internal Ollama API response shape — not exposed to callers
file sealed class OllamaApiResponse
{
    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("message")]
    public OllamaApiMessage? Message { get; init; }
}

file sealed class OllamaApiMessage
{
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; init; } = string.Empty;
}
