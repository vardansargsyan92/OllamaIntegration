namespace OllamaIntegration.Api.Models;

/// <summary>
/// Identifies the modality of an AI request/response.
/// Only Text is active. Image modalities are reserved for future use.
/// </summary>
public enum ModalityType
{
    Text = 0,
    // TextToImage = 1,   // reserved
    // ImageToImage = 2,  // reserved
    // ImageToText = 3,   // reserved
}
