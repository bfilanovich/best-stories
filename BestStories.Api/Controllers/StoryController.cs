using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BestStories.Api.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace BestStories.Api.Controllers;

[Route("v1/stories")]
[ApiController]
public class StoryController : ControllerBase
{
	[HttpGet("top")]
	public Task<ActionResult<TopStoryApiDto[]>> Get([Range(1, 500)]int count = 15)
	{
		return Task.FromResult((ActionResult<TopStoryApiDto[]>)Ok(GetSampleData(count)));
	}

	private static TopStoryApiDto[] GetSampleData(int count) => Enumerable.Range(1, count)
		.Select(x => new TopStoryApiDto
		{
			Title = $"Title {x}",
			Uri = $"www.example.com/story/{x}",
			PostedBy = $"Author {x}",
			CommentCount = x,
			Score = x,
			Time = DateTime.Now
		})
		.ToArray();
}
