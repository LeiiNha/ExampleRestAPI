using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExampleWebApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace ExampleWebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] UserCredentials credentials)
    {
        if (credentials is { Username: "admin", Password: "password" })
        {
           return Ok(new { Token = GenerateJwtToken() });
        }
        return Unauthorized();
    }

    private string GenerateJwtToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("This is my super uper duper secret key that noone will know");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "admin")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    private static readonly List<TodoItem> TodoItems =
    [
        new TodoItem { Id = 1, Name = "Item 1", IsComplete = false },
        new TodoItem { Id = 2, Name = "Item 2", IsComplete = false },
        new TodoItem { Id = 3, Name = "Item 3", IsComplete = false },
        new TodoItem { Id = 4, Name = "Item 4", IsComplete = true },
        new TodoItem { Id = 5, Name = "Item 5", IsComplete = false },
        new TodoItem { Id = 6, Name = "Item 6", IsComplete = false },
        new TodoItem { Id = 7, Name = "Item 7", IsComplete = true }
    ];

    [HttpGet]
    public ActionResult<IEnumerable<TodoItem>> Get()
    {
        return TodoItems;
    }

    [HttpGet("{id}")]
    public ActionResult<TodoItem> Get(int id)
    {
        var todoItem = TodoItems.Find(item => item.Id == id);
        if (todoItem == null)
        {
            return NotFound();
        }
        return todoItem;
    }

    [HttpPost]
    public IActionResult Post(TodoItem todoItem)
    {
        TodoItems.Add(todoItem);
        return CreatedAtAction(nameof(Get), new { id = todoItem.Id }, todoItem);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, TodoItem todoItem)
    {
        var existingTodoItem = TodoItems.Find(item => item.Id == id);
        if (existingTodoItem == null)
        {
            return NotFound();
        }
        existingTodoItem.Name = todoItem.Name;
        existingTodoItem.IsComplete = todoItem.IsComplete;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var todoItemToRemove = TodoItems.Find(item => item.Id == id);
        if (todoItemToRemove == null)
        {
            return NotFound();
        }
        TodoItems.Remove(todoItemToRemove);
        return NoContent();
    }
}