﻿using Microsoft.EntityFrameworkCore;
using N9.Data.Entities;

namespace N9.Data.Context;

public class BooksDbContext : DbContext
{
    public BooksDbContext()
    {
    }

    public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(e => e.Author)
                .WithMany(e => e.Books)
                .HasForeignKey(e => e.AuthorId);
        });
    }
}