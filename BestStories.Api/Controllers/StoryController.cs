using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using BestStories.Api.ApiModels;
using BestStories.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace BestStories.Api.Controllers;

[ApiController]
[Route("v1/stories")]
[Produces(MediaTypeNames.Application.Json)]
public class StoryController(HackerNewsStoryService service) : ControllerBase
{
	[HttpGet("top")]
	public async Task<ActionResult<TopStoryApiDto[]>> Get([Range(1, 200)] int count = 15, CancellationToken cancellationToken = default)
	{
		IReadOnlyCollection<TopStoryApiDto> stories = await service.GetTopStoriesAsync(count, cancellationToken);

		return Ok(stories);
	}
}
