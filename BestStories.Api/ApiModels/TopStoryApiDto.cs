using System;

namespace BestStories.Api.ApiModels;

public class TopStoryApiDto
{
	public string Title { get; set; } = string.Empty;
	public string Uri { get; set; } = string.Empty;
	public string PostedBy { get; set; } = string.Empty;
	public DateTimeOffset Time { get; set; }
	public int Score { get; set; }
	public int CommentCount { get; set; }
}
