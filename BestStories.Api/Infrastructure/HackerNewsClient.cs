using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BestStories.Api.Infrastructure.Abstractions;
using Microsoft.Extensions.Options;

namespace BestStories.Api.Infrastructure;

public class HackerNewsClient(HttpClient client, IOptions<HackerNewsClientOptions> options) : IHackerNewsClient
{
	public async Task<long[]> GetBestStoryIdsAsync(CancellationToken cancellationToken = default)
	{
		long[]? items = await client.GetFromJsonAsync<long[]>("v0/beststories.json", cancellationToken)
			.ConfigureAwait(false);

		return items ?? [];
	}

	public async Task<HackerNewsStoryDto> GetStoryAsync(long id, CancellationToken cancellationToken = default)
	{
		HackerNewsStoryDto? item = await client
			.GetFromJsonAsync<HackerNewsStoryDto>($"v0/item/{id}.json", cancellationToken)
			.ConfigureAwait(false);
		if (item is null)
		{
			throw new HttpRequestException("Story not found", null, HttpStatusCode.NotFound);
		}

		return item;
	}

	public async Task<IReadOnlyCollection<HackerNewsStoryDto>> GetStoriesAsync(IReadOnlyCollection<long> ids,
		CancellationToken cancellationToken = default)
	{
		if (ids.Count == 0)
		{
			return [];
		}

		var stories = new ConcurrentBag<HackerNewsStoryDto>();
		var parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = options.Value.MaxDegreeOfParallelism,
			CancellationToken = cancellationToken
		};

		await Parallel.ForEachAsync(ids, parallelOptions, async (id, ct) =>
			{
				HackerNewsStoryDto story = await GetStoryAsync(id, ct)
					.ConfigureAwait(false);

				stories.Add(story);
			})
			.ConfigureAwait(false);

		return stories;
	}
}
