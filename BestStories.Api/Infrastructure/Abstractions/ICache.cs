using System.Diagnostics.CodeAnalysis;

namespace BestStories.Api.Infrastructure.Abstractions;

public interface ICache
{
	bool TryGetValue<T>(object key, [NotNullWhen(true)] out T? value);
	void Set<T>(object key, T value);
}
