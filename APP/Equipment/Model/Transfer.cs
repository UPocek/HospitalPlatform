using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Transfer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("date")]
    [JsonPropertyName("date")]
    public string Date { get; set; }

    [BsonElement("room1")]
    [JsonPropertyName("room1")]
    public string Room1 { get; set; }

    [BsonElement("room2")]
    [JsonPropertyName("room2")]
    public string Room2 { get; set; }

    [BsonElement("done")]
    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [BsonElement("equipment")]
    [JsonPropertyName("equipment")]
    public List<Equipment> Equipment { get; set; }
}


