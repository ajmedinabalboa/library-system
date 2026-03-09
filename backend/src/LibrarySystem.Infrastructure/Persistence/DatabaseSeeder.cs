using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if we already have data
        if (context.Users.Any())
        {
            return; // Database has been seeded
        }

        var passwordHasher = new PasswordHasher<User>();

        // Seed Users
        var adminUser = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "admin@library.com",
            Name = "Admin User",
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

        var testUser = new User
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Email = "user@library.com",
            Name = "Test User",
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };
        testUser.PasswordHash = passwordHasher.HashPassword(testUser, "User123!");

        context.Users.AddRange(adminUser, testUser);

        // Seed Authors
        var authors = new[]
        {
            new Author
            {
                Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                Name = "Robert C. Martin",
                CreatedAt = DateTime.UtcNow
            },
            new Author
            {
                Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                Name = "Martin Fowler",
                CreatedAt = DateTime.UtcNow
            },
            new Author
            {
                Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                Name = "Eric Evans",
                CreatedAt = DateTime.UtcNow
            },
            new Author
            {
                Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                Name = "Kent Beck",
                CreatedAt = DateTime.UtcNow
            },
            new Author
            {
                Id = Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                Name = "Andrew Hunt",
                CreatedAt = DateTime.UtcNow
            }
        };
        context.Authors.AddRange(authors);

        // Seed Books
        var books = new[]
        {
            new Book
            {
                Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                PublicationDate = new DateTime(2008, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Book
            {
                Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"),
                Title = "Clean Architecture: A Craftsman's Guide to Software Structure",
                PublicationDate = new DateTime(2017, 9, 20, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Book
            {
                Id = Guid.Parse("b3333333-3333-3333-3333-333333333333"),
                Title = "Refactoring: Improving the Design of Existing Code",
                PublicationDate = new DateTime(1999, 7, 8, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Book
            {
                Id = Guid.Parse("b4444444-4444-4444-4444-444444444444"),
                Title = "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                PublicationDate = new DateTime(2003, 8, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Book
            {
                Id = Guid.Parse("b5555555-5555-5555-5555-555555555555"),
                Title = "Test Driven Development: By Example",
                PublicationDate = new DateTime(2002, 11, 18, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Book
            {
                Id = Guid.Parse("b6666666-6666-6666-6666-666666666666"),
                Title = "The Pragmatic Programmer: Your Journey to Mastery",
                PublicationDate = new DateTime(1999, 10, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        context.Books.AddRange(books);

        // Seed BookAuthors
        var bookAuthors = new[]
        {
            new BookAuthor { BookId = Guid.Parse("b1111111-1111-1111-1111-111111111111"), AuthorId = Guid.Parse("a1111111-1111-1111-1111-111111111111") },
            new BookAuthor { BookId = Guid.Parse("b2222222-2222-2222-2222-222222222222"), AuthorId = Guid.Parse("a1111111-1111-1111-1111-111111111111") },
            new BookAuthor { BookId = Guid.Parse("b3333333-3333-3333-3333-333333333333"), AuthorId = Guid.Parse("a2222222-2222-2222-2222-222222222222") },
            new BookAuthor { BookId = Guid.Parse("b3333333-3333-3333-3333-333333333333"), AuthorId = Guid.Parse("a4444444-4444-4444-4444-444444444444") },
            new BookAuthor { BookId = Guid.Parse("b4444444-4444-4444-4444-444444444444"), AuthorId = Guid.Parse("a3333333-3333-3333-3333-333333333333") },
            new BookAuthor { BookId = Guid.Parse("b5555555-5555-5555-5555-555555555555"), AuthorId = Guid.Parse("a4444444-4444-4444-4444-444444444444") },
            new BookAuthor { BookId = Guid.Parse("b6666666-6666-6666-6666-666666666666"), AuthorId = Guid.Parse("a5555555-5555-5555-5555-555555555555") }
        };
        context.BookAuthors.AddRange(bookAuthors);

        await context.SaveChangesAsync();
    }
}


