using System;
using BestStories.Api.Application;
using BestStories.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddScoped<HackerNewsStoryService>();

services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(client =>
{
	client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/");
});

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
