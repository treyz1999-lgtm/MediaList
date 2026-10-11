using System.Security.Cryptography;
using MediaWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MediaWebApp.Data;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        await SeedUser("test1", "Movies & Shows", [
            ("Spirited Away", "Movie", "Fantasy", 10),
            ("The Matrix", "Movie", "Science fiction", 9),
            ("Everything Everywhere All at Once", "Movie", "Adventure", 9),
            ("Arcane", "TV Show", "Animation", 9)
        ]);
        await SeedUser("test2", "Books & Comics", [
            ("Dune", "Book", "Science fiction", 9),
            ("The Hobbit", "Book", "Fantasy", 8),
            ("Watchmen", "Comic", "Superhero", 9),
            ("Saga", "Comic", "Science fiction", 8)
        ]);
        await SeedUser("test3", "Games & Anime", [
            ("Hollow Knight", "Game", "Adventure", 9),
            ("Stardew Valley", "Game", "Simulation", 9),
            ("Cowboy Bebop", "Anime", "Science fiction", 9),
            ("Fullmetal Alchemist: Brotherhood", "Anime", "Fantasy", 10)
        ]);

        async Task SeedUser(string displayName, string listName,
            (string Title, string Type, string Genre, int Rating)[] items)
        {
            var email = $"{displayName}@example.invalid";
            if (await users.FindByEmailAsync(email) is not null) return;

            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var password = Convert.ToHexString(RandomNumberGenerator.GetBytes(24)) + "aA1!";
            var result = await users.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Could not create {displayName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            db.UserProfiles.Add(new UserProfile { UserId = user.Id, DisplayName = displayName });
            db.MediaLists.Add(new MediaList
            {
                OwnerId = user.Id, Name = listName, IsPublic = true,
                Entries = items.Select(item => new MediaEntry
                {
                    OwnerId = user.Id, Title = item.Title, MediaType = item.Type,
                    Genre = item.Genre, Rating = item.Rating
                }).ToList()
            });
            await db.SaveChangesAsync();
        }
    }
}
