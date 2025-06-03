using MongoDB.Driver;                 // MongoDB
using TodoApi.Models;                // TodoItem model

namespace TodoApi.Services
{
    // Class for handling CRUD operations for Todo items
    public class TodoService
    {
        private readonly IMongoCollection<TodoItem> _todoCollection;

        //connects to the Mongodb collection
        public TodoService(IConfiguration config)
        {
            // Read MongoDB settings
            var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();

            // Create a MongoDB client and get the target database
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            // Get the collection for TodoItem objects
            _todoCollection = database.GetCollection<TodoItem>(settings.CollectionName);

            // Ensure the "Title" field is unique by creating an index on it
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexKeys = Builders<TodoItem>.IndexKeys.Ascending(todo => todo.Title);
            _todoCollection.Indexes.CreateOne(new CreateIndexModel<TodoItem>(indexKeys, indexOptions));
        }

        // Get all todo items
        public async Task<List<TodoItem>> GetAsync() =>
            await _todoCollection.Find(_ => true).ToListAsync();

        // Get a single todo item by its ID
        public async Task<TodoItem?> GetAsync(string id) =>
            await _todoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        // Insert a new todo item into the collection
        public async Task CreateAsync(TodoItem newItem) =>
            await _todoCollection.InsertOneAsync(newItem);

        // Update an existing todo item by ID
        public async Task UpdateAsync(string id, TodoItem updatedItem) =>
            await _todoCollection.ReplaceOneAsync(x => x.Id == id, updatedItem);

        // Delete a todo item by ID
        public async Task DeleteAsync(string id) =>
            await _todoCollection.DeleteOneAsync(x => x.Id == id);

        // Retrieve an item
        public async Task SaveChangesAsync(string id) =>
            await _todoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        // Update the `IsComplete` status field of a todo item
        public async Task UpdateStatusAsync(string id, string status)
        {
            var update = Builders<TodoItem>.Update.Set(x => x.IsComplete, status);
            await _todoCollection.UpdateOneAsync(x => x.Id == id, update);
        }

    }
}
