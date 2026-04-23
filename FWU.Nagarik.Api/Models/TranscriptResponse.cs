using System.Text.Json.Serialization;
using FWU.Nagarik.Api.ViewModels;

namespace FWU.Nagarik.Api.Models;

public class TranscriptResponse
{
    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("transcript")]
    public TranscriptViewModel? Transcript { get; set; }
}