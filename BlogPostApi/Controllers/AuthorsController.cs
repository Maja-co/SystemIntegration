using Microsoft.AspNetCore.Mvc;
using BlogPostApi.Models;
using System.Collections.Generic;
using System.Linq;
using BlogPostApi.Service;

namespace BlogPostApi.Controllers;

[Route("api/authors")]
[ApiController]
public class AuthorsController : ControllerBase {
    private readonly BlogDataService _dataService;

    public AuthorsController(BlogDataService dataService) {
        _dataService = dataService;
    }

    // ------------- PUT --------------------
    // Opdaterer en forfatter
    [HttpPut("{id}")]
    public IActionResult PutAuthor(long id, Author updatedAuthor) {
        var author = _dataService.GetAuthorById(id);
        if (author == null) return NotFound("Forfatteren findes ikke.");
        updatedAuthor.Id = id;
        _dataService.UpdateAuthor(updatedAuthor);

        return NoContent();
    }

    // ------------- GET --------------------
    // Henter alle blogindlæg for en specifik forfatter
    [HttpGet("{authorId}/posts")]
    public ActionResult<IEnumerable<BlogPost>> GetPostsByAuthor(long authorId) {
        var author = _dataService.GetAuthorById(authorId);
        if (author == null) return NotFound("Forfatteren findes ikke.");
        var posts = _dataService.GetPostsByAuthor(authorId);
        return Ok(posts);
    }

    // Henter alle forfattere
    [HttpGet]
    public ActionResult<IEnumerable<Author>> GetAuthors() {
        return Ok(_dataService.GetAllAuthors());
    }

    // Henter en specifik forfatter
    [HttpGet("{id}")]
    public ActionResult<Author> GetAuthor(long id) {
        var author = _dataService.GetAuthorById(id);
        if (author == null) return NotFound("Forfatteren findes ikke.");
        return Ok(author);
    }


    // ------------- POST --------------------
    // Opretter et nyt blogindlæg tilknyttet en forfatter
    [HttpPost("{authorId}/posts")]
    public ActionResult<BlogPost> PostBlogPostForAuthor(long authorId, BlogPost post) {
        var author = _dataService.GetAuthorById(authorId);
        if (author == null) return NotFound("Forfatteren findes ikke.");
        post.AuthorId = authorId;
        var createdPost = _dataService.AddPost(post);
        
        return Created($"/api/posts/{createdPost.Id}", createdPost);
    }


    // Opretter en ny forfatter
    [HttpPost]
    public ActionResult<Author> PostAuthor(Author author) {
        var createdAuthor = _dataService.AddAuthor(author);
        return CreatedAtAction(nameof(GetAuthor), new { id = createdAuthor.Id }, createdAuthor);
    }

    // ------------ DELETE --------------------
    // Sletter en forfatter
    [HttpDelete("{id}")]
    public IActionResult DeleteAuthor(long id) {
        var author = _dataService.GetAuthorById(id);
        if (author == null) return NotFound();
        _dataService.DeleteAuthor(id);
        return NoContent();
    }
}