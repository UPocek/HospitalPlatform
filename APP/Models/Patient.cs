using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Models
{
    [BsonIgnoreExtraElements]
    public class Patient
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id {get; set;}

        [BsonElement("firstName")]
        [JsonPropertyName("firstName")]
        public string firstName {get; set;}

        [BsonElement("lastName")]
        [JsonPropertyName("lastName")]
        public string dateAndlastName {get; set;}

        [BsonElement("role")]
        [JsonPropertyName("role")]
        public string role {get; set;}

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string email {get; set;}

        [BsonElement("password")]
        [JsonPropertyName("password")]
        public string password {get; set;}

        [BsonElement("active")]
        [JsonPropertyName("active")]
        public string active {get; set;}

        [BsonElement("id")]
        [JsonPropertyName("id")]
        public int id {get; set;}

        [BsonElement("medicalRecord")]
        [JsonPropertyName("medicalRecord")]
        public MedicalRecord medicalRecord {get; set;}

        [BsonElement("examinationHistory")]
        [JsonPropertyName("examinationHistory")]
        public List<ExaminationHistoryEntry>? examinationHistory {get; set;}=new List<ExaminationHistoryEntry>();

    }
}