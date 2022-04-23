using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class MedicalInstruction
{
    [BsonElement("date")]
    [JsonPropertyName("date")]
    public string date { get; set; }

    [BsonElement("doctor")]
    [JsonPropertyName("doctor")]
    public int doctorId { get; set; }
}
