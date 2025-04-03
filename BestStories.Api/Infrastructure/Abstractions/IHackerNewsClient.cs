using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BestStories.Api.Infrastructure.Abstractions;

public interface IHackerNewsClient
{
	Task<long[]> GetBestStoryIdsAsync(CancellationToken cancellationToken = default);

	Task<HackerNewsStoryDto> GetStoryAsync(long id, CancellationToken cancellationToken = default);

	// TODO: Get rid of this API
	Task<IReadOnlyCollection<HackerNewsStoryDto>> GetStoriesAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
}
