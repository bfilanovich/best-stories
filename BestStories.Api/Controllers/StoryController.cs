using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using BestStories.Api.ApiModels;
using BestStories.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace BestStories.Api.Controllers;

[Route("v1/stories")]
[ApiController]
public class StoryController(HackerNewsStoryService service) : ControllerBase
{
	[HttpGet("top")]
	public async Task<ActionResult<TopStoryApiDto[]>> Get([Range(1, 200)] int count = 15, CancellationToken cancellationToken = default)
	{
		IReadOnlyCollection<TopStoryApiDto> stories = await service.GetTopStoriesAsync(count, cancellationToken);

		return Ok(stories);
	}
}
