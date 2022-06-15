using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Drug
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [BsonElement("ingredients")]
    [JsonPropertyName("ingredients")]
    public List<string> Ingredients { get; set; }

    [BsonElement("status")]
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [BsonElement("comment")]
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}
