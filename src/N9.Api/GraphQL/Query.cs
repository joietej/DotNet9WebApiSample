using N9.Services;
using N9.Services.Models;

namespace N9.Api.GraphQL;

public class Query
{
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<BookModel>> GetBooks([Service] IBooksService booksService) =>
        (await booksService.GetBooksAsync()).AsQueryable();
}