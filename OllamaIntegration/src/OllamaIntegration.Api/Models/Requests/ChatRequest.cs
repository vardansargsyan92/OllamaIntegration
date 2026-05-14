using System.ComponentModel.DataAnnotations;

namespace OllamaIntegration.Api.Models.Requests;

public sealed class ChatRequest : AiRequest
{
    /// <summary>Ordered list of conversation turns.</summary>
    [Required]
    [MinLength(1, ErrorMessage = "At least one message is required.")]
    public List<ChatMessage> Messages { get; init; } = [];
}

public sealed class ChatMessage
{
    /// <summary>Allowed values: system | user | assistant</summary>
    [Required]
    [RegularExpression("^(system|user|assistant)$",
        ErrorMessage = "Role must be 'system', 'user', or 'assistant'.")]
    public string Role { get; init; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "Content cannot be empty.")]
    public string Content { get; init; } = string.Empty;
}
