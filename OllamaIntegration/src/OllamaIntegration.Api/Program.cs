using System.Text.Json;

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

app.MapPost("/api/chat", async (ChatRequest request, IHttpClientFactory clientFactory) =>
{
    var client = clientFactory.CreateClient("ollama");

    var ollamaRequest = new
    {
        model = "llama3.2:3b",
        stream = false,
        messages = request.Messages
    };

    var response = await client.PostAsJsonAsync("/api/chat", ollamaRequest);
    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
    return Results.Ok(result);
})
.WithName("Chat")
.WithOpenApi();

app.Run();

record ChatMessage(string Role, string Content);
record ChatRequest(List<ChatMessage> Messages);
