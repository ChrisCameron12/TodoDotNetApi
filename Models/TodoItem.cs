using MongoDB.Bson;                            
using MongoDB.Bson.Serialization.Attributes;   

namespace TodoApi.Models
{
    //Single Todo item stored in MongoDB
    public class TodoItem
    {
        // Marks this property as the document's unique identifier (_id in MongoDB)
        [BsonId]
        
        // Allows the Id to be represented as a string in the C# model, 
        // but stored as ObjectId in MongoDB
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Specifies the field name in MongoDB should be "title"
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        // Stores the completion status of the todo item (e.g., "pending", "complete")
        public string IsComplete { get; set; } = string.Empty;
    }
}
