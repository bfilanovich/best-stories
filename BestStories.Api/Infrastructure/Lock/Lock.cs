using System;
using System.Diagnostics.CodeAnalysis;
using BestStories.Api.Infrastructure.Abstractions.Lock;

namespace BestStories.Api.Infrastructure.Lock;

public sealed class Lock : ILock
{
	private readonly Action? _unlock;

	private Lock(string key, bool isLocked, Action? unlock = null)
	{
		Key = key;
		IsLocked = isLocked;

		if (isLocked && unlock == null)
		{
			throw new ArgumentNullException(nameof(unlock), "Unlock callback is required if locked");
		}

		_unlock = unlock;
	}

	public string Key { get; }

	[MemberNotNullWhen(true, nameof(_unlock))]
	public bool IsLocked { get; private set; }

	public static Lock Locked(string key, Action unlockCallback) => new(key, true, unlockCallback);

	public static Lock UnLocked(string key) => new(key, false);

	public void Dispose()
	{
		if (IsLocked)
		{
			_unlock();
			IsLocked = false;
		}
	}
}
