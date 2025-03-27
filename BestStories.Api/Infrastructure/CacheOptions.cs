using System;

namespace BestStories.Api.Infrastructure;

public class CacheOptions
{
	public const string Section = "Cache";

	public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(3);
	public TimeSpan PrefetchInterval { get; set; } = TimeSpan.FromMinutes(5);
}
