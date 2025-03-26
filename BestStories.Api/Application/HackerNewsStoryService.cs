using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BestStories.Api.ApiModels;

namespace BestStories.Api.Application;

public class HackerNewsStoryService(IHackerNewsClient hackerNewsClient)
{
	public async Task<IReadOnlyCollection<TopStoryApiDto>> GetTopStoriesAsync(
		int count,
		CancellationToken cancellationToken = default)
	{
		if (count is < 1 or > 200)
		{
			throw new ArgumentException($"Count must be between 1 and 200. Count: {count}", nameof(count));
		}

		long[] bestIds = await hackerNewsClient.GetBestStoriesAsync(cancellationToken);
		long[] topIds = bestIds.Take(count).ToArray();

		IReadOnlyCollection<HackerNewsStoryDto> betsStories = await hackerNewsClient.GetStoriesAsync(topIds, cancellationToken)
				.ConfigureAwait(false);

		return betsStories
			.Select(MapToStoryApiDto)
			.OrderByDescending(x => x.Score)
			.ToArray();
	}

	// Might be replaced with AutoMapper or a self-developed response mapper.
	private static TopStoryApiDto MapToStoryApiDto(HackerNewsStoryDto story) =>
		new()
		{
			Title = story.Title,
			Uri = story.Uri,
			PostedBy = story.PostedBy,
			CommentCount = story.CommentCount,
			Score = story.Score,
			Time = story.Time
		};
}
