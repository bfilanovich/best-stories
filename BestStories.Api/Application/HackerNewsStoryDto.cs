using System.Text.Json.Serialization;

namespace BestStories.Api.Application;

public class HackerNewsStoryDto
{
	public long Id { get; set; }
	public string Title { get; set; } = string.Empty;

	[JsonPropertyName("url")]
	public string Uri { get; set; } = string.Empty;

	[JsonPropertyName("by")]
	public string PostedBy { get; set; } = string.Empty;

	public long Time { get; set; }

	public int Score { get; set; }

	public int CommentCount { get; set; }
}
