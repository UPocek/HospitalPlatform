using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class ExaminationHistoryEntry
{
    [BsonElement("date")]
    [JsonPropertyName("date")]
    public string date {get; set;}

    [BsonElement("type")]
    [JsonPropertyName("type")]
    public string type {get; set;}
}