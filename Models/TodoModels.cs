using Amazon.DynamoDBv2.DataModel;
using System;

namespace TodoMvcNet8_Final.Models;
public class TodoItem
{
    public string Id { get; set; } 
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

[DynamoDBTable("TodoLists")]
public class TodoList
{
    [DynamoDBHashKey]
    public string OwnerId { get; set; }

    [DynamoDBRangeKey]
    public string ListId { get; set; }

    public string Title { get; set; }

    public List<TodoItem> Items { get; set; } = new();
}
