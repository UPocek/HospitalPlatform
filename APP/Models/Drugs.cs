using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Drugs
{
    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string name { get; set; }

    [BsonElement("ingredients")]
    [JsonPropertyName("ingredients")]
    public List<string> ingredients { get; set; }

    [BsonElement("status")]
    [JsonPropertyName("status")]
    public string status { get; set; }
}
