using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Purchase
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }
    
    [BsonElement("deadline")]
    [JsonPropertyName("deadline")]
    public string Deadline { get; set; }

    [BsonElement("what")]
    [JsonPropertyName("what")]
    public List<Equipment> What { get; set; } = new List<Equipment>();

    [BsonElement("done")]
    [JsonPropertyName("done")]
    public bool Done { get; set; } 

}
