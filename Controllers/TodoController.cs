using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoMvcNet8_Final.Data;
using TodoMvcNet8_Final.Models;

namespace TodoMvcNet8_Final.Controllers;

[Authorize]
public class TodoController : Controller
{
    private readonly TodoRepository _repo;

    public TodoController(TodoRepository repo)
    {
        _repo = repo;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.Email);
    }

    // LIST ALL TODO LISTS FOR CURRENT USER
    public async Task<IActionResult> Index(string? search)
    {
        var userId = GetUserId();

        var lists = await _repo.GetUserListsAsync(userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim().ToLower();

            lists = lists.Where(l =>
                l.Title.ToLower().Contains(q) ||
                l.Items.Any(i => i.Description.ToLower().Contains(q)))
                .ToList();

            ViewBag.Search = search;
        }

        return View(lists);
    }

    // CREATE PAGE
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // CREATE TODO LIST
    [HttpPost]
    public async Task<IActionResult> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return RedirectToAction(nameof(Index));

        var userId = GetUserId();

        await _repo.CreateListAsync(userId, title);

        return RedirectToAction(nameof(Index));
    }

    // EDIT PAGE
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = GetUserId();

        var list = await _repo.GetListByIdAsync(userId, id);

        if (list == null)
            return NotFound();

        return View(list);
    }

    // UPDATE LIST
    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, string title)
    {
        var userId = GetUserId();

        await _repo.UpdateListAsync(userId, id, title);

        return RedirectToAction(nameof(Index));
    }

    // DETAILS PAGE
    public async Task<IActionResult> Details(Guid id)
    {
        var userId = GetUserId();

        var list = await _repo.GetListByIdAsync(userId, id);

        if (list == null)
            return NotFound();

        return View(list);
    }

    // DELETE PAGE
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();

        var list = await _repo.GetListByIdAsync(userId, id);

        if (list == null)
            return NotFound();

        return View(list);
    }

    // CONFIRM DELETE
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var userId = GetUserId();

        await _repo.DeleteListAsync(userId, id);

        return RedirectToAction(nameof(Index));
    }

    // ADD ITEM
    [HttpPost]
    public async Task<IActionResult> AddItems(Guid listId, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return BadRequest("Description cannot be empty.");

        var userId = GetUserId();

        await _repo.AddItemAsync(userId, listId, description);

        return RedirectToAction(nameof(Details), new { id = listId });
    }

    // TOGGLE ITEM STATUS
    [HttpPost]
    public async Task<IActionResult> ToggleItem(Guid listId, Guid itemId)
    {
        var userId = GetUserId();

        await _repo.ToggleItemAsync(userId, listId, itemId);

        return RedirectToAction(nameof(Details), new { id = listId });
    }

    // DELETE ITEM
    [HttpPost]
    public async Task<IActionResult> DeleteItem(Guid listId, Guid itemId)
    {
        var userId = GetUserId();

        await _repo.DeleteItemAsync(userId, listId, itemId);

        return RedirectToAction(nameof(Details), new { id = listId });
    }

    // EDIT ITEM
    [HttpPost]
    public async Task<IActionResult> EditItem(Guid listId, Guid itemId, string title)
    {
        var userId = GetUserId();

        if (!string.IsNullOrWhiteSpace(title))
        {
            await _repo.EditItemAsync(userId, listId, itemId, title);
        }

        return RedirectToAction(nameof(Details), new { id = listId });
    }

    // THEME SWITCH
    [HttpPost]
    public IActionResult SetTheme(string theme)
    {
        Response.Cookies.Append("Theme", theme, new CookieOptions
        {
            Expires = DateTime.Now.AddDays(30)
        });

        return Redirect(Request.Headers["Referer"].ToString());
    }
}