using System.Diagnostics.CodeAnalysis;
using BestStories.Api.Infrastructure.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BestStories.Api.Infrastructure;

public class Cache(IMemoryCache memoryCache, IOptions<CacheOptions> cacheOptions) : ICache
{
	public bool TryGetValue<T>(object key, [NotNullWhen(true)] out T? value) => memoryCache.TryGetValue(key, out value!);

	public void Set<T>(object key, T value)
	{
		MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
			.SetSlidingExpiration(cacheOptions.Value.SlidingExpiration);
		memoryCache.Set(key, value, cacheEntryOptions);
	}
}
