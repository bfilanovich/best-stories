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

	public Task<HackerNewsStoryDto?> GetStoryAsync(long id, CancellationToken cancellationToken = default)
	{
		return client.GetFromJsonAsync<HackerNewsStoryDto>($"v0//item/{id}.json", cancellationToken);
	}
}
