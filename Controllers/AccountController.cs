using System.Security.Claims;
using MediaWebApp.Data;
using MediaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaWebApp.Controllers;

[Authorize]
public class AccountController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Profile()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var name = await db.UserProfiles.Where(p => p.UserId == id)
            .Select(p => p.DisplayName).FirstOrDefaultAsync();
        return View(new ProfileInput { DisplayName = name ?? string.Empty });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileInput input)
    {
        if (string.IsNullOrWhiteSpace(input.DisplayName)) ModelState.AddModelError(nameof(input.DisplayName), "Enter a display name.");
        if (!ModelState.IsValid) return View(input);
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await db.UserProfiles.FindAsync(id);
        if (profile is null) db.UserProfiles.Add(new UserProfile { UserId = id, DisplayName = input.DisplayName.Trim() });
        else profile.DisplayName = input.DisplayName.Trim();
        await db.SaveChangesAsync();
        return RedirectToAction("Mine", "Media");
    }
}
