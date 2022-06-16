using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class FreeDay
{
    [BsonElement("status")]
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [BsonElement("from")]
    [JsonPropertyName("from")]
    public string From { get; set; }

    [BsonElement("to")]
    [JsonPropertyName("to")]
    public string To { get; set; }

     [BsonElement("reason")]
    [JsonPropertyName("reason")]
    public string Reason { get; set; }

}