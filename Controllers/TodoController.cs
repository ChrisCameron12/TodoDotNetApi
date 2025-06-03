using System.Text.Json;                         
using Microsoft.AspNetCore.Mvc;                
using TodoApi.Models;                           
using TodoApi.Services;                         

namespace TodoApi.Controllers
{
    [ApiController]                            
    [Route("api/[controller]")]   // Sets route to api/todo (based on controller name)
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService; 

        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        // GET /api/todo?sortBy=complete
        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> Get([FromQuery] string? sortBy)
        {
            var items = await _todoService.GetAsync(); // Get all todos

            if (!string.IsNullOrEmpty(sortBy))
            {
                // Sort items to show matched status first, then sort by title
                var sorted = items
                    .OrderByDescending(i => i.IsComplete == sortBy)
                    .ThenBy(i => i.Title)
                    .ToList();

                return Ok(sorted);
            }

            return Ok(items); // Return all items unsorted if no sortBy provided
        }

        // POST /api/todo
        [HttpPost]
        public async Task<IActionResult> Post(TodoItem newItem)
        {
            await _todoService.CreateAsync(newItem); // Insert new item into DB

            // Return 201 Created with location header
            return CreatedAtAction(nameof(Get), new { id = newItem.Id }, newItem);
        }

        // PUT /api/todo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, TodoItem updatedItem)
        {
            var existing = await _todoService.GetAsync(id);
            if (existing is null) return NotFound(); // If item doesn't exist, return 404

            updatedItem.Id = id; // Ensure ID stays the same
            await _todoService.UpdateAsync(id, updatedItem); // Replace document

            return NoContent(); // Return 204 No Content
        }

        // DELETE /api/todo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _todoService.GetAsync(id);
            if (existing is null) return NotFound(); // If item doesn't exist, return 404

            await _todoService.DeleteAsync(id); // Delete from DB
            return NoContent(); // Return 204 No Content
        }

        // PATCH /api/todo/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchStatus(string id, [FromBody] JsonElement patch)
        {
            // Check if "isComplete" is in the request body
            if (!patch.TryGetProperty("isComplete", out var status))
                return BadRequest("Missing isComplete property");

            var item = await _todoService.GetAsync(id);
            if (item == null) return NotFound(); // If item doesn't exist, return 404

            // Update the "isComplete" status of the todo item
            await _todoService.UpdateStatusAsync(id, status.GetString());

            return NoContent(); // Return 204 No Content
        }
    }
}
