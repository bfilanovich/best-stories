using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BestStories.Api.ApiModels;
using BestStories.Api.Infrastructure.Abstractions;
using BestStories.Api.Infrastructure.Abstractions.Lock;

namespace BestStories.Api.Application;

public class HackerNewsStoryService(IHackerNewsClient hackerNewsClient, ICache cache, ILockProvider lockProvider)
{
	public async Task<IReadOnlyCollection<TopStoryApiDto>> GetTopStoriesAsync(
		int count,
		CancellationToken cancellationToken = default)
	{
		if (count is < 1 or > 200)
		{
			throw new ArgumentException($"Count must be between 1 and 200. Count: {count}", nameof(count));
		}

		long[] bestIds = await hackerNewsClient.GetBestStoryIdsAsync(cancellationToken);
		long[] topIds = bestIds.Take(count).ToArray();
		IReadOnlyCollection<HackerNewsStoryDto> stories = await GetStoriesAsync(topIds, cancellationToken)
			.ConfigureAwait(false);

		return stories
			.Select(MapToStoryApiDto)
			.OrderByDescending(x => x.Score)
			.ToArray();
	}

	private async Task<IReadOnlyCollection<HackerNewsStoryDto>> GetStoriesAsync(long[] ids, CancellationToken cancellationToken)
	{
		Task<HackerNewsStoryDto>[] requestTasks = ids
			.Select(id => RequestAndCacheAsync(id, cancellationToken))
			.ToArray();

		return await Task.WhenAll(requestTasks).ConfigureAwait(false);
	}

	private async Task<HackerNewsStoryDto> RequestAndCacheAsync(long id, CancellationToken cancellationToken)
	{
		var attemptsCount = 3;
		HackerNewsStoryDto? story;
		string storyKey = CreateStoryKey(id);
		while (!cache.TryGetValue(storyKey, out story) && attemptsCount > 0)
		{
			attemptsCount--;
			await Task.Yield();
			using ILock storyLock = lockProvider.TryLock(storyKey);
			if (storyLock.IsLocked)
			{
				return await GetAndStoreAsync(id, cancellationToken).ConfigureAwait(false);
			}

			await Task.Delay(100, cancellationToken).ConfigureAwait(false);
		}

		return story ?? await GetAndStoreAsync(id, cancellationToken).ConfigureAwait(false);

		async Task<HackerNewsStoryDto> GetAndStoreAsync(long storyId, CancellationToken ct)
		{
			HackerNewsStoryDto dto = await hackerNewsClient.GetStoryAsync(storyId, ct)
				.ConfigureAwait(false);
			cache.Set(storyKey, dto);
			return dto;
		}
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

	private static string CreateStoryKey(long storyId) => $"{nameof(HackerNewsStoryService)}_{storyId}";
}
