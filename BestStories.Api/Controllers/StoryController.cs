using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BestStories.Api.ApiModels;
using BestStories.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace BestStories.Api.Controllers;

[Route("v1/stories")]
[ApiController]
public class StoryController(IHackerNewsClient hackerNewsClient) : ControllerBase
{
	[HttpGet("top")]
	public async Task<ActionResult<TopStoryApiDto[]>> Get([Range(1, 200)] int count = 15, CancellationToken cancellationToken = default)
	{
		var bestIds = await hackerNewsClient.GetBestStoriesAsync(cancellationToken);

		var top5 = bestIds.Take(1)
			.Select(x => hackerNewsClient.GetStoryAsync(x, cancellationToken))
			.ToArray();

		HackerNewsStoryDto?[] betsStories = await Task.WhenAll(top5);
		var response = betsStories
			.Select(x => new TopStoryApiDto
			{
				Title = x.Title,
				Uri = x.Uri,
				PostedBy = x.PostedBy,
				CommentCount = x.CommentCount,
				Score = x.Score,
				Time = x.Time
			})
			.OrderByDescending(x => x.Score)
			.ToArray();

		return Ok(response);
	}
}
