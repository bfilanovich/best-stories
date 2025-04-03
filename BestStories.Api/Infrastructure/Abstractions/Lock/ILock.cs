using System;

namespace BestStories.Api.Infrastructure.Abstractions.Lock;

public interface ILock : IDisposable
{
	public string Key { get; }
	public bool IsLocked { get; }
}
