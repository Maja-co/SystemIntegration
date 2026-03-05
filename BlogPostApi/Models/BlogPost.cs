namespace BlogPostApi.Models;

public class BlogPost {
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public long AuthorId { get; set; }
}