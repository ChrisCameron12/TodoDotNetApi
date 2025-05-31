using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;

        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

   [HttpGet]
public async Task<ActionResult<List<TodoItem>>> Get([FromQuery] string? sortBy)
{
    var items = await _todoService.GetAsyncStatus();

    if (!string.IsNullOrEmpty(sortBy))
    {
        // Move the selected status to the top
        var sorted = items
            .OrderByDescending(i => i.IsComplete == sortBy)
            .ThenBy(i => i.Title) // Optional: sort alphabetically within group
            .ToList();

        return Ok(sorted);
    }

    return Ok(items);
}

   

        [HttpPost]
        public async Task<IActionResult> Post(TodoItem newItem)
        {
            await _todoService.CreateAsync(newItem);
            return CreatedAtAction(nameof(Get), new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, TodoItem updatedItem)
        {
            var existing = await _todoService.GetAsync(id);
            if (existing is null) return NotFound();
            updatedItem.Id = id;
            await _todoService.UpdateAsync(id, updatedItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _todoService.GetAsync(id);
            if (existing is null) return NotFound();
            await _todoService.DeleteAsync(id);
            return NoContent();
        }
       [HttpPatch("{id}")]
        public async Task<IActionResult> PatchStatus(string id, [FromBody] JsonElement patch)
        {
            if (!patch.TryGetProperty("isComplete", out var status))
                return BadRequest("Missing isComplete property");

            var item = await _todoService.GetAsync(id);
            if (item == null) return NotFound();

            // Update status in DB
            await _todoService.UpdateStatusAsync(id, status.GetString());

            return NoContent();
        }

    }
}
