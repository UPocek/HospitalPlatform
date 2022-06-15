using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class DrugNotification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _Id { get; set; }

    [BsonElement("drug")]
    [JsonPropertyName("drug")]
    public string Drug { get; set; }

    [BsonElement("time")]
    [JsonPropertyName("time")]
    public string Time { get; set; }

    [BsonElement("paitent")]
    [JsonPropertyName("patient")]
    public string Patient { get; set; }

    [BsonElement("endDate")]
    [JsonPropertyName("endDate")]
    public string EndDate { get; set; }
}