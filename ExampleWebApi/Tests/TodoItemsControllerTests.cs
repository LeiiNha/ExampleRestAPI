namespace ExampleWebApi.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Model;
using ExampleWebApi; 


public class TodoItemsControllerTests(WebApplicationFactory<Startup> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Get_ReturnsSuccessAndCorrectContentType()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/todoitems");
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Post_ReturnsCreatedAndResource()
    {
        var client = factory.CreateClient();
        var todoItem = new { Name = "Test Todo Item", IsComplete = false };
        var content = new StringContent(JsonSerializer.Serialize(todoItem), Encoding.UTF8, "application/json");
        
        var response = await client.PostAsync("/api/todoitems", content);
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdTodoItem = JsonSerializer.Deserialize<TodoItem>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(createdTodoItem);
        Assert.Equal(todoItem.Name, createdTodoItem.Name);
        Assert.Equal(todoItem.IsComplete, createdTodoItem.IsComplete);
    }
}
