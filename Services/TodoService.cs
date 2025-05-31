using MongoDB.Driver;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService
    {
        private readonly IMongoCollection<TodoItem> _todoCollection;

        public TodoService(IConfiguration config)
        {
            var settings = config.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _todoCollection = database.GetCollection<TodoItem>(settings.CollectionName);

            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexKeys = Builders<TodoItem>.IndexKeys.Ascending(todo => todo.Title);
            _todoCollection.Indexes.CreateOne(new CreateIndexModel<TodoItem>(indexKeys, indexOptions));

        }

        public async Task<List<TodoItem>> GetAsync() =>
            await _todoCollection.Find(_ => true).ToListAsync();

        public async Task<TodoItem?> GetAsync(string id) =>
            await _todoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(TodoItem newItem) =>
            await _todoCollection.InsertOneAsync(newItem);

        public async Task UpdateAsync(string id, TodoItem updatedItem) =>
            await _todoCollection.ReplaceOneAsync(x => x.Id == id, updatedItem);

        public async Task DeleteAsync(string id) =>
            await _todoCollection.DeleteOneAsync(x => x.Id == id);

        public async Task SaveChangesAsync(string id) =>
            await _todoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task UpdateStatusAsync(string id, string status)
        {
            var update = Builders<TodoItem>.Update.Set(x => x.IsComplete, status);
            await _todoCollection.UpdateOneAsync(x => x.Id == id, update);
        }

  public async Task<List<TodoItem>> GetAsyncStatus() =>
    await _todoCollection.Find(_ => true).ToListAsync();

            
    }
}