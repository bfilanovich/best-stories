namespace BestStories.Api.Infrastructure.Abstractions.Lock;

// TODO: Implement API for requesting the clock.
public interface ILockProvider
{
	ILock TryLock(string key);
}
