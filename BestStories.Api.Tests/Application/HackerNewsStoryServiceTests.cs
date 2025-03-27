using AutoFixture;
using BestStories.Api.ApiModels;
using BestStories.Api.Application;
using BestStories.Api.Infrastructure.Abstractions;
using Moq;

namespace BestStories.Api.Tests.Application;

public class HackerNewsStoryServiceTests
{
	private readonly Fixture _fixture = new();
	private readonly HackerNewsStoryService _sut;
	private readonly Mock<IHackerNewsClient> _clientMock = new();
	private readonly Mock<ICache> _cacheMock = new();

	public HackerNewsStoryServiceTests()
	{
		_sut = new HackerNewsStoryService(_clientMock.Object, _cacheMock.Object);
	}

	[Fact]
	public async Task ShouldRequestProperAmountOfStories()
	{
		const int requestedCount = 3;
		const int totalCount = requestedCount + 5;
		HackerNewsStoryDto[] bestStories = _fixture.CreateMany<HackerNewsStoryDto>(totalCount).OrderByDescending(x => x.Score).ToArray();
		long[] bestStoryIds = bestStories.Select(x => x.Id).ToArray();
		_clientMock.Setup(x => x.GetBestStoryIdsAsync(CancellationToken.None))
			.ReturnsAsync(bestStoryIds);
		_clientMock.Setup(x => x.GetStoriesAsync(bestStoryIds.Take(requestedCount).ToArray(), CancellationToken.None))
			.ReturnsAsync(bestStories.Take(requestedCount).ToArray);

		IReadOnlyCollection<TopStoryApiDto> result = await _sut.GetTopStoriesAsync(requestedCount);

		Assert.Equal(requestedCount, result.Count);
	}
}
