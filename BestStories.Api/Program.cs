using System;
using System.Threading.RateLimiting;
using BestStories.Api.Application;
using BestStories.Api.BackgroundServices;
using BestStories.Api.Infrastructure;
using BestStories.Api.Infrastructure.Abstractions;
using BestStories.Api.Infrastructure.Abstractions.Lock;
using BestStories.Api.Infrastructure.HttpMessageHandlers;
using BestStories.Api.Infrastructure.Lock;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddHostedService<PrefetchBestStoriesTask>();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddMemoryCache();
services.AddTransient<ICache, Cache>();
services.AddScoped<HackerNewsStoryService>();
services.AddSingleton<RateLimiter>(_ =>
{
	HackerNewsClientOptions options = _.GetRequiredService<IOptions<HackerNewsClientOptions>>().Value;
	return new ConcurrencyLimiter(new ConcurrencyLimiterOptions { PermitLimit = options.ConcurrentRequestsLimit, QueueLimit = options.ConcurrentRequestQueueLimit });
});
services.AddSingleton<ILockProvider, LockProvider>();
services.Configure<HackerNewsClientOptions>(builder.Configuration.GetSection(HackerNewsClientOptions.Section));
services.Configure<CacheOptions>(builder.Configuration.GetSection(CacheOptions.Section));

services.AddTransient<HackerNewsRateLimitedHandler>();
services.AddHttpClient<IHackerNewsClient, HackerNewsClient>((sp, client) =>
	{
		HackerNewsClientOptions options = sp.GetRequiredService<IOptions<HackerNewsClientOptions>>().Value;
		client.BaseAddress = options.BaseUri ?? throw new InvalidOperationException("BaseUri isn't configured.");
	})
	.AddPolicyHandler((sp, _) => HttpPolicyExtensions.HandleTransientHttpError()
		.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(200),
			onRetry: (exception, _, retryCount, _) =>
			{
				var logger = sp.GetRequiredService<ILogger<HackerNewsClient>>();
				logger.LogWarning("Unsuccessful request to Hakcer News: {Message}. Retry Count: {RetryCount}.", exception.Exception.Message, retryCount);
			}))
	.AddHttpMessageHandler<HackerNewsRateLimitedHandler>();

WebApplication app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
