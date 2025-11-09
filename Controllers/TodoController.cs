using Microsoft.AspNetCore.Mvc;
using TodoMvcNet8_Final.Data;
using TodoMvcNet8_Final.Models;

namespace TodoMvcNet8_Final.Controllers;
//[ApiController]
//[Route("[controller]/[method]")]
public class TodoController : Controller
{
    private readonly TodoRepository _repo;
    public TodoController(TodoRepository repo) => _repo = repo;

    //[HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var lists = await _repo.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim().ToLower();
            lists = lists.Where(l => l.Title.ToLower().Contains(q) || l.Items.Any(i => i.Description.ToLower().Contains(q))).ToList();
            ViewBag.Search = search;
        }
        return View(lists);
    }


    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return RedirectToAction(nameof(Index));
        var lists = await _repo.GetAllAsync();
        lists.Add(new TodoList { Title = title.Trim() });
        await _repo.SaveAllAsync(lists);
        return RedirectToAction(nameof(Index));
    }


    //[HttpPut]
    public async Task<IActionResult> Edit(Guid id)
    {
        var lists = await _repo.GetAllAsync();
        var list = lists.FirstOrDefault(l => l.Id == id);
        if (list == null) return NotFound();
        return View(list);
    }

    [HttpPost("EditTest")]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, string title)
    {
        var lists = await _repo.GetAllAsync();
        var list = lists.FirstOrDefault(l => l.Id == id);
        if (list != null && !string.IsNullOrWhiteSpace(title))
        {
            list.Title = title.Trim();
            await _repo.SaveAllAsync(lists);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var lists = await _repo.GetAllAsync();
        var list = lists.FirstOrDefault(l => l.Id == id);
        if (list == null) return NotFound();
        return View(list);
    }


    public async Task<IActionResult> Delete(Guid id)
    {
        var lists = await _repo.GetAllAsync();
        var list = lists.FirstOrDefault(l => l.Id == id);
        if (list == null) return NotFound();
        return View(list);
    }

    [HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var lists = await _repo.GetAllAsync();
        lists.RemoveAll(l => l.Id == id);
        await _repo.SaveAllAsync(lists);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult AddItems(Guid listId, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return BadRequest("Description cannot be empty.");
        }

        // Assuming you have a repository or service to handle adding items
        TodoList todoList = _repo.GetById(listId);
        if (todoList == null)
        {
            return NotFound($"Todo list with ID {listId} not found.");
        }

        todoList.Items.Add(new TodoItem
        {
            Id = Guid.NewGuid(),
            Description = description,
            IsCompleted = false
        });

        _repo.Update(todoList);

        return RedirectToAction("Index");
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleItem(Guid listId, Guid itemId)
    {
        var lists = await _repo.GetAllAsync();
        var item = lists.SelectMany(l => l.Items.Where(i => i.Id == itemId && l.Id == listId)).FirstOrDefault();
        if (item != null)
        {
            item.IsCompleted = !item.IsCompleted;
            await _repo.SaveAllAsync(lists);
        }
        return RedirectToAction(nameof(Details), new { id = listId });
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteItem(Guid listId, Guid itemId)
    {
        var lists = await _repo.GetAllAsync();
        var list = lists.FirstOrDefault(l => l.Id == listId);
        if (list != null)
        {
            list.Items.RemoveAll(i => i.Id == itemId);
            await _repo.SaveAllAsync(lists);
        }
        return RedirectToAction(nameof(Details), new { id = listId });
    }

    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> EditItem(Guid listId, Guid itemId, string title)
    {
        var lists = await _repo.GetAllAsync();
        var item = lists.SelectMany(l => l.Items.Where(i => i.Id == itemId && l.Id == listId)).FirstOrDefault();
        if (item != null && !string.IsNullOrWhiteSpace(title))
        {
            item.Description = title.Trim();
            await _repo.SaveAllAsync(lists);
        }
        return RedirectToAction(nameof(Details), new { id = listId });
    }

    //[HttpPost, ActionName("AddItems")]
    //public async Task<IActionResult> AddItems(Guid id, string title)
    //{
    //    if (string.IsNullOrWhiteSpace(title)) return RedirectToAction(nameof(Index));
    //    var lists = await _repo.GetAllAsync();
    //    lists.Add(new TodoList { Title = title.Trim() });
    //    await _repo.SaveAllAsync(lists);
    //    return RedirectToAction(nameof(Index));
    //}

    [HttpPost]
    public IActionResult SetTheme(string theme)
    {
        Response.Cookies.Append("Theme", theme, new CookieOptions { Expires = DateTime.Now.AddDays(30) });
        return Redirect(Request.Headers["Referer"].ToString());
    }
}
