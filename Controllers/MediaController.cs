using System.Security.Claims;
using MediaWebApp.Data;
using MediaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaWebApp.Controllers;

public class MediaController(ApplicationDbContext db) : Controller
{
    private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    [Authorize]
    public async Task<IActionResult> Mine()
    {
        var userId = CurrentUserId!;
        return View(await db.MediaLists.Where(l => l.OwnerId == userId)
            .Include(l => l.Entries).OrderBy(l => l.Name).ToListAsync());
    }

    [Authorize]
    public IActionResult CreateList() => View(new ListInput());

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateList(ListInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name)) ModelState.AddModelError(nameof(input.Name), "Enter a list name.");
        if (!ModelState.IsValid) return View(input);
        var list = new MediaList { OwnerId = CurrentUserId!, Name = input.Name.Trim() };
        db.MediaLists.Add(list);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(List), new { id = list.Id });
    }

    [Authorize]
    public async Task<IActionResult> EditList(int id)
    {
        var list = await OwnList(id);
        if (list is null) return NotFound();
        ViewBag.ListId = id;
        return View(new ListInput { Name = list.Name });
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditList(int id, ListInput input)
    {
        var list = await OwnList(id);
        if (list is null) return NotFound();
        if (string.IsNullOrWhiteSpace(input.Name)) ModelState.AddModelError(nameof(input.Name), "Enter a list name.");
        if (!ModelState.IsValid) { ViewBag.ListId = id; return View(input); }
        list.Name = input.Name.Trim();
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(List), new { id });
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        var list = await OwnList(id);
        if (list is null) return NotFound();
        if (!list.IsPublic && !await db.UserProfiles.AnyAsync(p => p.UserId == CurrentUserId))
        {
            TempData["ListError"] = "Set your display name before publishing a list.";
            return RedirectToAction("Profile", "Account");
        }
        list.IsPublic = !list.IsPublic;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(List), new { id });
    }

    [Authorize]
    public async Task<IActionResult> Create(int listId)
    {
        if (await OwnList(listId) is null) return NotFound();
        ViewBag.ListId = listId;
        return View(new MediaEntryInput());
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int listId, MediaEntryInput input)
    {
        if (await OwnList(listId) is null) return NotFound();
        if (!ModelState.IsValid) { ViewBag.ListId = listId; return View(input); }
        db.MediaEntries.Add(new MediaEntry
        {
            ListId = listId, OwnerId = CurrentUserId!, Title = input.Title.Trim(),
            MediaType = input.MediaType.Trim(), Genre = input.Genre?.Trim(),
            Rating = input.Rating, CoverImageUrl = input.CoverImageUrl?.Trim()
        });
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(List), new { id = listId });
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var entry = await OwnEntry(id);
        if (entry is null) return NotFound();
        ViewBag.EntryId = id;
        ViewBag.ListId = entry.ListId;
        return View(new MediaEntryInput { Title = entry.Title, MediaType = entry.MediaType,
            Genre = entry.Genre, Rating = entry.Rating, CoverImageUrl = entry.CoverImageUrl });
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MediaEntryInput input)
    {
        var entry = await OwnEntry(id);
        if (entry is null) return NotFound();
        if (!ModelState.IsValid) { ViewBag.EntryId = id; ViewBag.ListId = entry.ListId; return View(input); }
        entry.Title = input.Title.Trim();
        entry.MediaType = input.MediaType.Trim();
        entry.Genre = input.Genre?.Trim();
        entry.Rating = input.Rating;
        entry.CoverImageUrl = input.CoverImageUrl?.Trim();
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(List), new { id = entry.ListId });
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await OwnEntry(id);
        if (entry is null) return NotFound();
        db.MediaEntries.Remove(entry);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(List), new { id = entry.ListId });
    }

    public async Task<IActionResult> List(int id)
    {
        var list = await db.MediaLists.Include(l => l.Entries).FirstOrDefaultAsync(l => l.Id == id);
        if (list is null || (!list.IsPublic && list.OwnerId != CurrentUserId)) return NotFound();
        ViewBag.IsOwner = list.OwnerId == CurrentUserId;
        ViewBag.OwnerName = await db.UserProfiles.Where(p => p.UserId == list.OwnerId)
            .Select(p => p.DisplayName).FirstOrDefaultAsync() ?? "Member";
        return View(list);
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> UserList(string id)
    {
        if (!await db.Users.AnyAsync(u => u.Id == id)) return NotFound();
        var isOwner = id == CurrentUserId;
        var lists = await db.MediaLists.Where(l => l.OwnerId == id && (l.IsPublic || isOwner))
            .Include(l => l.Entries).OrderBy(l => l.Name).ToListAsync();
        if (!isOwner && lists.Count == 0) return NotFound();
        ViewBag.OwnerName = await db.UserProfiles.Where(p => p.UserId == id)
            .Select(p => p.DisplayName).FirstOrDefaultAsync() ?? "Member";
        ViewBag.IsOwner = isOwner;
        return View(lists);
    }

    private Task<MediaList?> OwnList(int id) => db.MediaLists
        .FirstOrDefaultAsync(l => l.Id == id && l.OwnerId == CurrentUserId);

    private Task<MediaEntry?> OwnEntry(int id) => db.MediaEntries
        .FirstOrDefaultAsync(e => e.Id == id && e.OwnerId == CurrentUserId);
}
