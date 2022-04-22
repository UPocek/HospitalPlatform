using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

[BsonIgnoreExtraElements]
public class MedicalRecord
{
    [BsonElement("height")]
    [JsonPropertyName("height")]
    public int height { get; set; }

    [BsonElement("wieght")]
    [JsonPropertyName("weight")]
    public int weight { get; set; }

    [BsonElement("bloodType")]
    [JsonPropertyName("bloodType")]
    public string bloodType { get; set; }

    [BsonElement("diseases")]
    [JsonPropertyName("diseases")]
    public List<string> diseases { get; set; }

    [BsonElement("alergies")]
    [JsonPropertyName("alergies")]
    public List<string> alergies { get; set; }

    [BsonElement("drugs")]
    [JsonPropertyName("drugs")]
    [BsonRepresentation(BsonType.ObjectId)]
    public List<Prescription> prescriptions { get; set; }

    [BsonElement("examinations")]
    [JsonPropertyName("examinations")]
    public List<int> examinations { get; set; }

    [BsonElement("medicalInstructioins")]
    [JsonPropertyName("medicalInstructioins")]
    public List<MedicalInstructioin> medicalInstructioins { get; set; } = null;
}