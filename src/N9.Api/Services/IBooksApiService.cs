using Microsoft.AspNetCore.Http.HttpResults;
using N9.Services.Models;

namespace N9.Api.Services;

public interface IBooksApiService
{
    Task<Results<Ok<IEnumerable<BookModel>>, InternalServerError>> GetBooksAsync();
}