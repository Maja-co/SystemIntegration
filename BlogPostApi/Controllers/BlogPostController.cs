using Microsoft.AspNetCore.Mvc;
using BlogPostApi.Models;
using System.Linq;
using BlogPostApi.Service;

namespace BlogPostApi.Controllers;

[Route("api/posts")]
[ApiController]
public class BlogPostController : ControllerBase {
    private readonly BlogDataService _dataService;

    public BlogPostController(BlogDataService dataService) {
        _dataService = dataService;
    }

    // ------------- GET --------------------
    // Henter et specifikt blogindlæg
    [HttpGet("{id}")]
    public ActionResult<BlogPost> GetBlogPost(long id) {
        var blogPost = _dataService.GetPostById(id);
        if (blogPost == null) return NotFound("Blogposten findes ikke.");
        return Ok(blogPost);
    }

    // ------------- PUT --------------------
    // Opdaterer et blogindlæg
    [HttpPut("{id}")]
    public IActionResult PutBlogPost(long id, BlogPost updatedPost) {
        var blogPost = _dataService.GetPostById(id);
        if (blogPost == null) return NotFound("Blogposten findes ikke.");

        updatedPost.Id = id;
        _dataService.UpdatePost(updatedPost);

        return NoContent();
    }


    // ------------ DELETE --------------------
    // Sletter et blogindlæg
    [HttpDelete("{id}")]
    public IActionResult DeleteBlogPost(long id) {
        var blogPost = _dataService.GetPostById(id);
        if (blogPost == null) return NotFound("Blogposten findes ikke.");

        _dataService.DeletePost(id);
        return NoContent();
    }
}