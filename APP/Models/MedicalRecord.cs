using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Models
{
    [BsonIgnoreExtraElements]
    public class MedicalRecord
    {
        [BsonElement("height")]
        [JsonPropertyName("height")]
        public int height {get; set;}

        [BsonElement("weight")]
        [JsonPropertyName("weight")]
        public int weight {get; set;}

        [BsonElement("bloodType")]
        [JsonPropertyName("bloodType")]
        public string bloodType {get; set;}

        [BsonElement("diseases")]
        [JsonPropertyName("diseases")]
        public List<string> diseases {get; set;} = new List<string>();

        [BsonElement("alergies")]
        [JsonPropertyName("alergies")]
        public List<string> alergies {get; set;} = new List<string>();

        [BsonElement("drugs")]
        [JsonPropertyName("drugs")]
        public List<Prescription>? prescriptions {get; set;} = new List<Prescription>();

        [BsonElement("examinations")]
        [JsonPropertyName("examinations")]
        public List<int>? examinations {get; set;} = new List<int>();

        [BsonElement("medicalInstructioins")]
        [JsonPropertyName("medicalInstructioins")]
        public List<MedicalInstruction>? medicalInstructioins {get; set;} = new List<MedicalInstruction>();
    }
}