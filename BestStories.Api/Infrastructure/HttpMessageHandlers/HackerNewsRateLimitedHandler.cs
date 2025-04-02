using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace BestStories.Api.Infrastructure.HttpMessageHandlers;

public sealed class HackerNewsRateLimitedHandler(RateLimiter limiter) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		using RateLimitLease lease = await limiter.AcquireAsync(1, cancellationToken)
			.ConfigureAwait(false);
		if (lease.IsAcquired)
		{
			return await base.SendAsync(request, cancellationToken)
				.ConfigureAwait(false);
		}

		return new HttpResponseMessage(HttpStatusCode.TooManyRequests);
	}
}
