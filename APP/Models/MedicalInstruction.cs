using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class MedicalInstruction
{
    [BsonElement("startDate")]
    [JsonPropertyName("startDate")]
    public string startDate { get; set; }

    [BsonElement("endDate")]
    [JsonPropertyName("endDate")]
    public string endtDate { get; set; }

    [BsonElement("drug")]
    [JsonPropertyName("drug")]
    public string drugName { get; set; }

    [BsonElement("doctor")]
    [JsonPropertyName("doctor")]
    public int doctorId { get; set; }
}
