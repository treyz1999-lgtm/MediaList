using MediaWebApp.Data;
using MediaWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaWebApp.Controllers;

public class SearchController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? q)
    {
        var model = new SearchResults { Query = q?.Trim() ?? string.Empty };
        if (model.Query.Length == 0) return View(model);
        var term = $"%{model.Query}%";
        model.Media = await db.MediaEntries.AsNoTracking().Include(e => e.List)
            .Where(e => e.List.IsPublic &&
                (EF.Functions.Like(e.Title, term) || EF.Functions.Like(e.MediaType, term)
                    || (e.Genre != null && EF.Functions.Like(e.Genre, term))))
            .OrderBy(e => e.Title).Take(50).ToListAsync();
        model.Users = await db.UserProfiles.AsNoTracking()
            .Where(p => db.MediaLists.Any(l => l.OwnerId == p.UserId && l.IsPublic)
                && EF.Functions.Like(p.DisplayName, term))
            .OrderBy(p => p.DisplayName)
            .Select(p => new UserSearchResult { Id = p.UserId, DisplayName = p.DisplayName })
            .Take(50).ToListAsync();
        model.Lists = await db.MediaLists.AsNoTracking()
            .Where(l => l.IsPublic && EF.Functions.Like(l.Name, term))
            .OrderBy(l => l.Name).Take(50).ToListAsync();
        return View(model);
    }
}
