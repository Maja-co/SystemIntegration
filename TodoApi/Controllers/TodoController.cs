using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoController : ControllerBase {
    private readonly TodoContext _context;

    public TodoController(TodoContext context) {
        _context = context;
    }

    // GET: 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems() {
        return await _context.TodoItems.ToListAsync();
    }

    // POST: 
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem) {
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
    }


    // GET: 
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodoItem(long id) {
        var todoItem = await _context.TodoItems.FindAsync(id);

        if (todoItem == null) {
            return NotFound();
        }

        return todoItem;
    }

    // PUT: 
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem) {
        if (id != todoItem.Id) {
            return BadRequest();
        }

        _context.Entry(todoItem).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {
            if (!_context.TodoItems.Any(e => e.Id == id)) {
                return NotFound();
            }
            else {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE:
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id) {
        var todoItem = await _context.TodoItems.FindAsync(id);
        if (todoItem == null) {
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}