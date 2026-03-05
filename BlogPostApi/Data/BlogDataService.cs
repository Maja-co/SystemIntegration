using BlogPostApi.Models;

namespace BlogPostApi.Service;

public class BlogDataService {
    private static List<Author> _authors = new List<Author>();
    private static List<BlogPost> _posts = new List<BlogPost>();
    private static long _nextAuthorId = 1;
    private static long _nextPostId = 1;

    // --- GET METODER ---
    public List<Author> GetAllAuthors() => _authors;
    public Author? GetAuthorById(long id) => _authors.FirstOrDefault(a => a.Id == id);
    public List<BlogPost> GetAllPosts() => _posts;
    public BlogPost? GetPostById(long id) => _posts.FirstOrDefault(p => p.Id == id);
    public List<BlogPost> GetPostsByAuthor(long authorId) => _posts.Where(p => p.AuthorId == authorId).ToList();

    // ------------- POST --------------------
        // At oprette
    public Author AddAuthor(Author newAuthor) {
        newAuthor.Id = _nextAuthorId; // 1. Giv id
        _authors.Add(newAuthor); // 2. Tilføj til liste
        _nextAuthorId++; // 3. Tæl id op til næste gang
        return newAuthor;
    }

    public BlogPost AddPost(BlogPost newPost) {
        newPost.Id = _nextPostId;
        _posts.Add(newPost);
        _nextPostId++;
        return newPost;
    }

    // ------------- PUT --------------------
        // At opdatere
    public void UpdateAuthor(Author updatedAuthor) {
        var existing = _authors.FirstOrDefault(a => a.Id == updatedAuthor.Id);
        if (existing != null) {
            existing.Name = updatedAuthor.Name;
        }
    }

    public void UpdatePost(BlogPost updatedPost) {
        var existing = _posts.FirstOrDefault(p => p.Id == updatedPost.Id);
        if (existing != null) {
            existing.Title = updatedPost.Title;
            existing.Content = updatedPost.Content;
        }
    }

    // ------------ DELETE --------------------
    public void DeleteAuthor(long id) {
        var author = _authors.FirstOrDefault(a => a.Id == id);
        if (author != null) {
            _authors.Remove(author);
        }
    }

    public void DeletePost(long id) {
        var post = _posts.FirstOrDefault(p => p.Id == id);
        if (post != null) {
            _posts.Remove(post);
        }
    }
}