using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

[BsonIgnoreExtraElements]
public class FreeDayRequest
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _Id { get; set; }

    [BsonElement("doctorId")]
    [JsonPropertyName("doctorId")]
    public int DoctorId { get; set; }

    [BsonElement("requestDay")]
    [JsonPropertyName("requestDay")]
    public string? RequestDay { get; set; }

    [BsonElement("startDay")]
    [JsonPropertyName("startDay")]
    public string StartDay { get; set; }

    [BsonElement("duration")]
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [BsonElement("status")]
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [BsonElement("reason")]
    [JsonPropertyName("reason")]
    public string Reason { get; set; }

    [BsonElement("urgent")]
    [JsonPropertyName("urgent")]
    public bool Urgent { get; set; }
}