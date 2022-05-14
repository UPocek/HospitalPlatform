using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class MedicalInstruction
{
    [BsonElement("startDate")]
    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }

    [BsonElement("endDate")]
    [JsonPropertyName("endDate")]
    public string EndtDate { get; set; }

    [BsonElement("drug")]
    [JsonPropertyName("drug")]
    public string DrugName { get; set; }

    [BsonElement("doctor")]
    [JsonPropertyName("doctor")]
    public int DoctorId { get; set; }
}
