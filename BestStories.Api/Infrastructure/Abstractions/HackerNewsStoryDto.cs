using System;
using System.Text.Json.Serialization;
using BestStories.Api.Serialization;

namespace BestStories.Api.Infrastructure.Abstractions;

public class HackerNewsStoryDto
{
	public long Id { get; set; }
	public string Title { get; set; } = string.Empty;

	[JsonPropertyName("url")]
	public string Uri { get; set; } = string.Empty;

	[JsonPropertyName("by")]
	public string PostedBy { get; set; } = string.Empty;

	[JsonConverter(typeof(DateTimeFromUnixEpochJsonConverter))]
	public DateTime Time { get; set; }

	public int Score { get; set; }

	[JsonPropertyName("descendants")]
	public int CommentCount { get; set; }
}
