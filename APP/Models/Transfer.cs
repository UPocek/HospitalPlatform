using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Transfer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    [BsonElement("date")]
    [JsonPropertyName("date")]
    public string date { get; set; }

    [BsonElement("room1")]
    [JsonPropertyName("room1")]
    public string room1 { get; set; }

    [BsonElement("room2")]
    [JsonPropertyName("room2")]
    public string room2 { get; set; }

    [BsonElement("done")]
    [JsonPropertyName("done")]
    public bool done { get; set; }

    [BsonElement("equipment")]
    [JsonPropertyName("equipment")]
    public List<Equipment> equipment { get; set; }
}


