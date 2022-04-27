using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class DrugIngredients
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    [BsonElement("ingredients")]
    [JsonPropertyName("ingredients")]
    public List<string> ingredients { get; set; }
}
