using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BestStories.Api.ApiModels;
using BestStories.Api.Infrastructure.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace BestStories.Api.Application;

public class HackerNewsStoryService(IHackerNewsClient hackerNewsClient, IMemoryCache memoryCache)
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
		IReadOnlyCollection<HackerNewsStoryDto> cached = GetCached(topIds);
		long[] notFoundIds = topIds.Except(cached.Select(x => x.Id)).ToArray();
		IReadOnlyCollection<HackerNewsStoryDto> newStories = [];
		if (notFoundIds.Length != 0)
		{
			newStories = await RequestAndCacheAsync(notFoundIds, cancellationToken)
				.ConfigureAwait(false);
		}

		return cached
			.Concat(newStories)
			.Select(MapToStoryApiDto)
			.OrderByDescending(x => x.Score)
			.ToArray();
	}

	private List<HackerNewsStoryDto> GetCached(long[] ids)
	{
		var result = new List<HackerNewsStoryDto>(ids.Length);
		foreach (long id in ids)
		{
			if (memoryCache.TryGetValue(id, out HackerNewsStoryDto? cached))
			{
				result.Add(cached!);
			}
		}

		return result;
	}

	private async Task<IReadOnlyCollection<HackerNewsStoryDto>> RequestAndCacheAsync(long[] ids, CancellationToken cancellationToken)
	{
		IReadOnlyCollection<HackerNewsStoryDto> betsStories = await hackerNewsClient.GetStoriesAsync(ids, cancellationToken)
			.ConfigureAwait(false);

		foreach (HackerNewsStoryDto item in betsStories)
		{
			MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
				.SetSlidingExpiration(TimeSpan.FromSeconds(120));
			memoryCache.Set(item.Id, item, cacheEntryOptions);
		}

		return betsStories;
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
