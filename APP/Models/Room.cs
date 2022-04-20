using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;

public class Room
{
    public Room(string name, string type, bool inRenovation, List<Equipment> equipment)
    {
        this.name = name;
        this.type = type;
        this.inRenovation = inRenovation;
        this.equipment = equipment;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string name { get; set; }

    [BsonElement("type")]
    [JsonPropertyName("type")]
    public string type { get; set; }

    [BsonElement("inRenovation")]
    [JsonPropertyName("inRenovation")]
    public bool inRenovation { get; set; }

    [BsonElement("equipment")]
    [JsonPropertyName("equipment")]
    public List<Equipment> equipment { get; set; } = new List<Equipment>();
}