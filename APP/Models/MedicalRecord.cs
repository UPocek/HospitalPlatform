using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;


[BsonIgnoreExtraElements]
public class MedicalRecord
{
    [BsonElement("height")]
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [BsonElement("weight")]
    [JsonPropertyName("weight")]
    public int Weight { get; set; }

    [BsonElement("bloodType")]
    [JsonPropertyName("bloodType")]
    public string BloodType { get; set; }

    [BsonElement("diseases")]
    [JsonPropertyName("diseases")]
    public List<string>? Diseases { get; set; } = new List<string>();

    [BsonElement("alergies")]
    [JsonPropertyName("alergies")]
    public List<string>? Alergies { get; set; } = new List<string>();

    [BsonElement("drugs")]
    [JsonPropertyName("drugs")]
    public List<Prescription>? Prescriptions { get; set; } = new List<Prescription>();

    [BsonElement("examinations")]
    [JsonPropertyName("examinations")]
    public List<int>? Examinations { get; set; } = new List<int>();

    [BsonElement("referrals")]
    [JsonPropertyName("referrals")]
    public List<Referral>? Referrals{ get; set; } = new List<Referral>();


    [BsonElement("medicalInstructions")]
    [JsonPropertyName("medicalInstructions")]
    public List<MedicalInstruction>? MedicalInstructions { get; set; } = new List<MedicalInstruction>();

}

