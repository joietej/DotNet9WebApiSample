using N9.Api.Services;
using N9.Data.Repositories;
using N9.Services;

namespace N9.Api.Extensions;

public static class Dependencies
{
    public static WebApplicationBuilder AddConfig(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IBookRepository, BookRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

        builder.Services.AddScoped<IBooksService, BooksService>();

        builder.Services.AddScoped<IBooksApiService, BooksApiService>();

        return builder;
    }
}