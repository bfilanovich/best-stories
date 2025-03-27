using System;
using BestStories.Api.Application;
using BestStories.Api.Infrastructure;
using BestStories.Api.Infrastructure.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddScoped<HackerNewsStoryService>();
services.Configure<HackerNewsClientOptions>(builder.Configuration.GetSection(HackerNewsClientOptions.Section));

services.AddHttpClient<IHackerNewsClient, HackerNewsClient>((sp, client) =>
{
	HackerNewsClientOptions options = sp.GetRequiredService<IOptions<HackerNewsClientOptions>>().Value;
	client.BaseAddress = options.BaseUri ?? throw new InvalidOperationException("BaseUri isn't configured.");
});

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
