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

	public HackerNewsStoryServiceTests() =>
		_sut = new HackerNewsStoryService(_clientMock.Object, _cacheMock.Object);

	[Fact]
	public async Task ShouldReturnTopCountOfStories()
	{
		const int requestedCount = 3;
		const int totalCount = requestedCount + 5;
		HackerNewsStoryDto[] bestStories = _fixture.CreateMany<HackerNewsStoryDto>(totalCount).OrderByDescending(x => x.Score).ToArray();
		long[] bestStoryIds = bestStories.Select(x => x.Id).ToArray();
		long[] topRequestedIds = bestStoryIds.Take(requestedCount).ToArray();
		HackerNewsStoryDto[] topRequested = bestStories.Take(requestedCount).ToArray();
		_clientMock.Setup(x => x.GetBestStoryIdsAsync(CancellationToken.None))
			.ReturnsAsync(bestStoryIds);
		_clientMock.Setup(x => x.GetStoriesAsync(topRequestedIds, CancellationToken.None))
			.ReturnsAsync(topRequested);

		IReadOnlyCollection<TopStoryApiDto> result = await _sut.GetTopStoriesAsync(requestedCount);

		Assert.Equal(requestedCount, result.Count);
		Assert.True(topRequested.Select(x => x.Score).SequenceEqual(result.Select(x => x.Score)));
		foreach ((TopStoryApiDto actual, HackerNewsStoryDto expected) in result.Zip(topRequested, (a, b) => (a, b )))
		{
			Assert.Equal(expected.Title, actual.Title);
			Assert.Equal(expected.Uri, actual.Uri);
			Assert.Equal(expected.PostedBy, actual.PostedBy);
			Assert.Equal(expected.Time, actual.Time);
			Assert.Equal(expected.Score, actual.Score);
			Assert.Equal(expected.CommentCount, actual.CommentCount);
		}
		_clientMock.VerifyAll();
		_cacheMock.Verify(x => x.TryGetValue(It.IsIn(topRequestedIds), out It.Ref<HackerNewsStoryDto>.IsAny!), Times.Exactly(requestedCount));
		_cacheMock.Verify(x => x.Set(It.IsIn(topRequestedIds), It.IsAny<HackerNewsStoryDto>()), Times.Exactly(requestedCount));
	}
}
