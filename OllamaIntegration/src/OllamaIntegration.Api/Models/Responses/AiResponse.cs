namespace OllamaIntegration.Api.Models.Responses;

/// <summary>Base response shared by all modalities.</summary>
public abstract class AiResponse
{
    public ModalityType Modality { get; init; } = ModalityType.Text;
    public bool Success { get; init; }
    public string? Error { get; init; }
}
