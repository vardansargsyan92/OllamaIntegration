namespace OllamaIntegration.Api.Models.Requests;

/// <summary>Base request shared by all modalities.</summary>
public abstract class AiRequest
{
    public ModalityType Modality { get; init; } = ModalityType.Text;
}
