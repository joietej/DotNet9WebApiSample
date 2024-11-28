namespace N9.Data.Entities;

public class Book : IEntity
{
    public required string Title { get; set; }
    public required int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public int Id { get; set; }
}