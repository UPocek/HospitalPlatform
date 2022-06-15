using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Patient
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _Id { get; set; }

    [BsonElement("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [BsonElement("firstName")]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [BsonElement("lastName")]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [BsonElement("email")]
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [BsonElement("role")]
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [BsonElement("password")]
    [JsonPropertyName("password")]
    public string Password { get; set; }

    [BsonElement("active")]
    [JsonPropertyName("active")]
    public string Active { get; set; }

    [BsonElement("medicalRecord")]
    [JsonPropertyName("medicalRecord")]
    public MedicalRecord MedicalRecord { get; set; }

    [BsonElement("examinationHistory")]
    [JsonPropertyName("examinationHistory")]
    public List<ExaminationHistoryEntry>? ExaminationHistory { get; set; } = new List<ExaminationHistoryEntry>();
}
