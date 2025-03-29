using System;
using System.Threading;
using System.Threading.Tasks;
using BestStories.Api.Application;
using BestStories.Api.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BestStories.Api.BackgroundServices;

public class PrefetchBestStoriesTask(IServiceScopeFactory serviceScopeFactory,
	IOptions<CacheOptions> cacheOptions,
	ILogger<PrefetchBestStoriesTask> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using IServiceScope scope = serviceScopeFactory.CreateScope();
			var storyService = scope.ServiceProvider.GetRequiredService<HackerNewsStoryService>();
			try
			{
				await PrefetchStories(storyService, stoppingToken);
			}
			catch (Exception e)
			{
				logger.LogError(e, e.Message);
				throw;
			}

			await Task.Delay(cacheOptions.Value.PrefetchInterval, stoppingToken);
		}
	}

	private async Task PrefetchStories(HackerNewsStoryService service, CancellationToken cancellationToken)
	{
		const int maxCount = 200;
		logger.LogInformation("Prefetching best stories...");
		await service.GetTopStoriesAsync(maxCount, cancellationToken);
	}
}
