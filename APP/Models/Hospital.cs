using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

[BsonIgnoreExtraElements]
public class Hospital
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }

    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string name { get; set; }

    [BsonElement("polls")]
    [JsonPropertyName("polls")]
    public List<Dictionary<string, string>> polls { get; set; }
}