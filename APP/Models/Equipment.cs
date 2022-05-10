using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Equipment
{
    public Equipment(string name, string type, int quantity)
    {
        this.Name = name;
        this.Type = type;
        this.Quantity = quantity;
    }

    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [BsonElement("type")]
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [BsonElement("quantity")]
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
