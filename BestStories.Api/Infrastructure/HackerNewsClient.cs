using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BestStories.Api.Application;

namespace BestStories.Api.Infrastructure;

public class HackerNewsClient(HttpClient client) : IHackerNewsClient
{
	public async Task<long[]> GetBestStoriesAsync(CancellationToken cancellationToken = default)
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
		var stories = new ConcurrentBag<HackerNewsStoryDto>();
		var options = new ParallelOptions { MaxDegreeOfParallelism = 25, CancellationToken = cancellationToken };
		await Parallel.ForEachAsync(ids, options, async (id, ct) =>
			{
				HackerNewsStoryDto story = await GetStoryAsync(id, ct)
					.ConfigureAwait(false);

				stories.Add(story);
			})
			.ConfigureAwait(false);

		return stories;
	}
}
