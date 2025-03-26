using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BestStories.Api.Application;

public interface IHackerNewsClient
{
	Task<long[]> GetBestStoriesAsync(CancellationToken cancellationToken = default);

	Task<HackerNewsStoryDto> GetStoryAsync(long id, CancellationToken cancellationToken = default);

	Task<IReadOnlyCollection<HackerNewsStoryDto>> GetStoriesAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);
}
