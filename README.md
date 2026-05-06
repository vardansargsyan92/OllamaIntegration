# OllamaIntegration

An ASP.NET Core 8 Web API that integrates with [Ollama](https://ollama.com) to provide a local LLM (Large Language Model) chat endpoint. Everything runs locally in Docker — no cloud API keys required.

## What is Ollama?

Ollama is an open-source tool that lets you run large language models locally on your own machine. It manages model downloads, GPU acceleration, and exposes a simple REST API to interact with models. Think of it as a self-hosted alternative to the OpenAI API.

This project uses the **llama3.2:3b** model — a lightweight 3-billion parameter model made by Meta, suitable for running on most machines without a GPU.

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) with Linux containers enabled
- At least **4 GB of free disk space** for the model download
- At least **4 GB of RAM** available to Docker

## Project Structure

```
OllamaIntegration/
├── src/
│   └── OllamaIntegration.Api/     # ASP.NET Core 8 minimal API
│       ├── Program.cs
│       ├── Dockerfile
│       └── appsettings.json
├── docker-compose.yml             # Orchestrates API + Ollama
└── README.md
```

## How to Run

```bash
docker-compose up --build
```

That single command will:
1. Build the API Docker image
2. Start the Ollama server
3. Pull the **llama3.2:3b** model (~2 GB, first run only)
4. Start the API once Ollama is healthy

> **Note:** The first run takes a few minutes because it downloads the model. Subsequent runs start instantly — the model is cached in a Docker volume (`ollama-data`).

Once running, the services are available at:

| Service | URL |
|---|---|
| API | http://localhost:8080 |
| Swagger UI | http://localhost:8080/swagger |
| Ollama (direct) | http://localhost:11434 |

## API Endpoints

### POST /api/chat

Sends a message to the llama3.2:3b model and returns its response.

**Request**

```json
{
  "messages": [
    {
      "role": "user",
      "content": "Explain what Redis is"
    }
  ]
}
```

**Example with curl**

```bash
curl --location 'http://localhost:8080/api/chat' \
--header 'Content-Type: application/json' \
--data '{
  "messages": [
    {
      "role": "user",
      "content": "Explain what Redis is"
    }
  ]
}'
```

**Multi-turn conversation example**

```json
{
  "messages": [
    { "role": "user", "content": "What is Docker?" },
    { "role": "assistant", "content": "Docker is a platform for running containers..." },
    { "role": "user", "content": "How does it differ from a virtual machine?" }
  ]
}
```

## Configuration

The Ollama base URL is configured via `appsettings.json` for local development and via an environment variable in Docker:

| Setting | Local (`appsettings.json`) | Docker (`docker-compose.yml`) |
|---|---|---|
| `Ollama:BaseUrl` | `http://localhost:11434` | `http://ollama:11434` |

To change the model, update the `model` field in `Program.cs`. Available models can be found at [ollama.com/library](https://ollama.com/library).

## Stopping the App

```bash
docker-compose down
```

To also remove the downloaded model and free up disk space:

```bash
docker-compose down -v
```
