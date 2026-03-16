using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using TodoMvcNet8_Final.Models;

namespace TodoMvcNet8_Final.Data;

public class TodoRepository
{
    private readonly DynamoDBContext _context;

    public TodoRepository(IAmazonDynamoDB dynamoDb)
    {
        _context = new DynamoDBContext(dynamoDb);
    }

    public async Task<List<TodoList>> GetUserListsAsync(string ownerId)
    {
        var query = _context.QueryAsync<TodoList>(ownerId);
        return await query.GetRemainingAsync();
    }

    public async Task<TodoList?> GetListByIdAsync(string ownerId, Guid listId)
    {
        return await _context.LoadAsync<TodoList>(ownerId, listId.ToString());
    }

    public async Task CreateListAsync(string ownerId, string title)
    {
        var list = new TodoList
        {
            OwnerId = ownerId,
            ListId = Guid.NewGuid().ToString(),
            Title = title
        };

        await _context.SaveAsync(list);
    }

    public async Task UpdateListAsync(string ownerId, Guid listId, string title)
    {
        var list = await _context.LoadAsync<TodoList>(ownerId, listId.ToString());

        if (list != null)
        {
            list.Title = title;
            await _context.SaveAsync(list);
        }
    }

    public async Task DeleteListAsync(string ownerId, Guid listId)
    {
        await _context.DeleteAsync<TodoList>(ownerId, listId.ToString());
    }

    public async Task AddItemAsync(string ownerId, Guid listId, string description)
    {
        var list = await _context.LoadAsync<TodoList>(ownerId, listId.ToString());

        if (list != null)
        {
            list.Items.Add(new TodoItem
            {
                Id = Guid.NewGuid().ToString(),
                Description = description,
                IsCompleted = false
            });

            await _context.SaveAsync(list);
        }
    }

    public async Task ToggleItemAsync(string ownerId, Guid listId, Guid itemId)
    {
        var list = await _context.LoadAsync<TodoList>(ownerId, listId.ToString());

        var item = list?.Items.FirstOrDefault(i => i.Id == itemId.ToString());

        if (item != null)
        {
            item.IsCompleted = !item.IsCompleted;
            await _context.SaveAsync(list);
        }
    }

    public async Task DeleteItemAsync(string ownerId, Guid listId, Guid itemId)
    {
        var list = await _context.LoadAsync<TodoList>(ownerId, listId.ToString());

        if (list != null)
        {
            list.Items.RemoveAll(i => i.Id == itemId.ToString());
            await _context.SaveAsync(list);
        }
    }

    public async Task EditItemAsync(string ownerId, Guid listId, Guid itemId, string title)
    {
        var list = await _context.LoadAsync<TodoList>(ownerId, listId.ToString());

        if (list == null)
            return;

        var item = list.Items.FirstOrDefault(i => i.Id == itemId.ToString());

        if (item != null)
        {
            item.Description = title.Trim();
            await _context.SaveAsync(list);
        }
    }
}