﻿namespace N9.Services.Models;

public record BookModel
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required AuthorModel Author { get; init; }
}