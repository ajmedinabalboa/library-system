using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Infrastructure.Persistence;

public static class SeedData
{
    // Pre-generated GUIDs for consistent seed data
    public static readonly Guid AdminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid TestUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static readonly Guid Author1Id = Guid.Parse("a1111111-1111-1111-1111-111111111111");
    public static readonly Guid Author2Id = Guid.Parse("a2222222-2222-2222-2222-222222222222");
    public static readonly Guid Author3Id = Guid.Parse("a3333333-3333-3333-3333-333333333333");
    public static readonly Guid Author4Id = Guid.Parse("a4444444-4444-4444-4444-444444444444");
    public static readonly Guid Author5Id = Guid.Parse("a5555555-5555-5555-5555-555555555555");

    public static readonly Guid Book1Id = Guid.Parse("b1111111-1111-1111-1111-111111111111");
    public static readonly Guid Book2Id = Guid.Parse("b2222222-2222-2222-2222-222222222222");
    public static readonly Guid Book3Id = Guid.Parse("b3333333-3333-3333-3333-333333333333");
    public static readonly Guid Book4Id = Guid.Parse("b4444444-4444-4444-4444-444444444444");
    public static readonly Guid Book5Id = Guid.Parse("b5555555-5555-5555-5555-555555555555");
    public static readonly Guid Book6Id = Guid.Parse("b6666666-6666-6666-6666-666666666666");

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedUsers(modelBuilder);
        SeedAuthors(modelBuilder);
        SeedBooks(modelBuilder);
        SeedBookAuthors(modelBuilder);
    }

    private static void SeedUsers(ModelBuilder modelBuilder)
    {
        var passwordHasher = new PasswordHasher<User>();

        var adminUser = new User
        {
            Id = AdminUserId,
            Email = "admin@library.com",
            Role = "Admin",
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

        var testUser = new User
        {
            Id = TestUserId,
            Email = "user@library.com",
            Role = "User",
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        testUser.PasswordHash = passwordHasher.HashPassword(testUser, "User123!");

        modelBuilder.Entity<User>().HasData(adminUser, testUser);
    }

    private static void SeedAuthors(ModelBuilder modelBuilder)
    {
        var authors = new[]
        {
            new Author
            {
                Id = Author1Id,
                Name = "Robert C. Martin",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Author
            {
                Id = Author2Id,
                Name = "Martin Fowler",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Author
            {
                Id = Author3Id,
                Name = "Eric Evans",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Author
            {
                Id = Author4Id,
                Name = "Kent Beck",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Author
            {
                Id = Author5Id,
                Name = "Andrew Hunt",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        modelBuilder.Entity<Author>().HasData(authors);
    }

    private static void SeedBooks(ModelBuilder modelBuilder)
    {
        var books = new[]
        {
            new Book
            {
                Id = Book1Id,
                Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                PublicationDate = new DateTime(2008, 8, 1),
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Book
            {
                Id = Book2Id,
                Title = "Clean Architecture: A Craftsman's Guide to Software Structure",
                PublicationDate = new DateTime(2017, 9, 20),
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Book
            {
                Id = Book3Id,
                Title = "Refactoring: Improving the Design of Existing Code",
                PublicationDate = new DateTime(1999, 7, 8),
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Book
            {
                Id = Book4Id,
                Title = "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                PublicationDate = new DateTime(2003, 8, 30),
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Book
            {
                Id = Book5Id,
                Title = "Test Driven Development: By Example",
                PublicationDate = new DateTime(2002, 11, 18),
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Book
            {
                Id = Book6Id,
                Title = "The Pragmatic Programmer: Your Journey to Mastery",
                PublicationDate = new DateTime(1999, 10, 30),
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        modelBuilder.Entity<Book>().HasData(books);
    }

    private static void SeedBookAuthors(ModelBuilder modelBuilder)
    {
        var bookAuthors = new[]
        {
            // Clean Code - Robert C. Martin
            new BookAuthor { BookId = Book1Id, AuthorId = Author1Id },
            
            // Clean Architecture - Robert C. Martin
            new BookAuthor { BookId = Book2Id, AuthorId = Author1Id },
            
            // Refactoring - Martin Fowler (with Kent Beck as co-author)
            new BookAuthor { BookId = Book3Id, AuthorId = Author2Id },
            new BookAuthor { BookId = Book3Id, AuthorId = Author4Id },
            
            // Domain-Driven Design - Eric Evans
            new BookAuthor { BookId = Book4Id, AuthorId = Author3Id },
            
            // Test Driven Development - Kent Beck
            new BookAuthor { BookId = Book5Id, AuthorId = Author4Id },
            
            // The Pragmatic Programmer - Andrew Hunt (assuming single author for simplicity)
            new BookAuthor { BookId = Book6Id, AuthorId = Author5Id }
        };

        modelBuilder.Entity<BookAuthor>().HasData(bookAuthors);
    }
}

