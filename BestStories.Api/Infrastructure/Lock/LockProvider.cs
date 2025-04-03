using System.Collections.Concurrent;
using BestStories.Api.Infrastructure.Abstractions.Lock;

namespace BestStories.Api.Infrastructure.Lock;

public sealed class LockProvider : ILockProvider
{
	private readonly ConcurrentDictionary<string, Lock> _locks = new();

	public ILock TryLock(string key)
	{
		var newLock = Lock.Locked(key, () => _locks.TryRemove(key, out _));
		return _locks.TryAdd(key, newLock) ? newLock : Lock.UnLocked(key);
	}
}
