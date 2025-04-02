using System;

namespace BestStories.Api.Infrastructure;

public class HackerNewsClientOptions
{
	public const string Section = "HackerNewsClient";

	public Uri? BaseUri { get; set; }
	public int MaxDegreeOfParallelism { get; set; }
	public int ConcurrentRequestsLimit { get; set; }
	public int ConcurrentRequestQueueLimit { get; set; }
}
