using System;
using BestStories.Api.Application;
using BestStories.Api.BackgroundServices;
using BestStories.Api.Infrastructure;
using BestStories.Api.Infrastructure.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddHostedService<PrefetchBestStoriesTask>();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddMemoryCache();
services.AddScoped<HackerNewsStoryService>();
services.Configure<HackerNewsClientOptions>(builder.Configuration.GetSection(HackerNewsClientOptions.Section));
services.Configure<CacheOptions>(builder.Configuration.GetSection(CacheOptions.Section));

services.AddHttpClient<IHackerNewsClient, HackerNewsClient>((sp, client) =>
	{
		HackerNewsClientOptions options = sp.GetRequiredService<IOptions<HackerNewsClientOptions>>().Value;
		client.BaseAddress = options.BaseUri ?? throw new InvalidOperationException("BaseUri isn't configured.");
	})
	.AddTransientHttpErrorPolicy(x =>
		x.WaitAndRetryAsync(3, retryNumber => TimeSpan.FromMilliseconds(200)));

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
