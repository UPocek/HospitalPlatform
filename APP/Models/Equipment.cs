using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Equipment
{
    public Equipment(string name, string type, int quantity)
    {
        this.name = name;
        this.type = type;
        this.quantity = quantity;
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

    [BsonElement("quantity")]
    [JsonPropertyName("quantity")]
    public int quantity { get; set; }
}


