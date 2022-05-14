using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;

public class Room
{
    public Room(string name, string type, bool inRenovation, List<Equipment> equipment)
    {
        this.Name = name;
        this.Type = type;
        this.InRenovation = inRenovation;
        this.Equipment = equipment;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [BsonElement("type")]
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [BsonElement("inRenovation")]
    [JsonPropertyName("inRenovation")]
    public bool InRenovation { get; set; }

    [BsonElement("equipment")]
    [JsonPropertyName("equipment")]
    public List<Equipment> Equipment { get; set; } = new List<Equipment>();
}