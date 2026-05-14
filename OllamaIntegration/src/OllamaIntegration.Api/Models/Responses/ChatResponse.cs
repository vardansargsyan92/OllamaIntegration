namespace OllamaIntegration.Api.Models.Responses;

public sealed class ChatResponse : AiResponse
{
    /// <summary>The assistant's reply message.</summary>
    public ChatReplyMessage? Message { get; init; }

    /// <summary>The model that generated the response.</summary>
    public string? Model { get; init; }
}

public sealed class ChatReplyMessage
{
    public string Role { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
}
