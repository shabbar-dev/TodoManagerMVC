using System.Text.Json;
using TodoMvcNet8_Final.Models;
using System.Threading;

namespace TodoMvcNet8_Final.Data;
public class TodoRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };
    private readonly SemaphoreSlim _lock = new(1,1);

    public TodoRepository(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", "todos.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
    }

    public async Task<List<TodoList>> GetAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var txt = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<TodoList>>(txt) ?? new();
        }
        finally { _lock.Release(); }
    }

    public async Task SaveAllAsync(List<TodoList> lists)
    {
        await _lock.WaitAsync();
        try
        {
            var txt = JsonSerializer.Serialize(lists, _opts);
            await File.WriteAllTextAsync(_filePath, txt);
        }
        finally { _lock.Release(); }
    }

    public  TodoList GetById(Guid id)
    {
        //try
        //{
            var txt = File.ReadAllText(_filePath);
            var response = JsonSerializer.Deserialize<List<TodoList>>(txt)?? new List<TodoList>();
            return response.First(x => x.Id == id);
        //}
        //finally { _lock.Release(); }
    }

    //public async Task update(TodoList lists)
    //{
    //    await _lock.WaitAsync();
    //    try
    //    {
    //        var txt = File.ReadAllText(_filePath);
    //        var response = JsonSerializer.Deserialize<List<TodoList>>(txt) ?? new List<TodoList>();
    //        var updateList = response.First(x => x.Id == lists.Id);
    //        response.Add(updateList);
    //        txt = JsonSerializer.Serialize(response, _opts);
    //        await File.WriteAllTextAsync(_filePath, txt);
    //    }
    //    finally { _lock.Release(); }
    //}


    public async Task Update(TodoList list)
    {
        await _lock.WaitAsync();
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var items = JsonSerializer.Deserialize<List<TodoList>>(json) ?? new List<TodoList>();

            var index = items.FindIndex(x => x.Id == list.Id);
            if (index != -1)
            {
                items[index] = list; // ? Replace existing item
            }
            else
            {
                items.Add(list); // Optionally add if not found
            }

            json = JsonSerializer.Serialize(items, _opts);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _lock.Release();
        }
    }

}
