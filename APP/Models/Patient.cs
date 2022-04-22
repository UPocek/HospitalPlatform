using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Models
{
    [BsonIgnoreExtraElements]
    public class Patient
    {

        public Patient(string firstName, string lastName, string role, string email, string password, string active, string id, MedicalRecord medicalRecord)
        {
        this.firstName = firstName;
        this.lastName = lastName;
        this.role = role;
        this.email = email;
        this.password = password;
        this.active = active;
        this.id = id;
        this.medicalRecord = medicalRecord;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id {get; set;}

        [BsonElement("firstName")]
        [JsonPropertyName("firstName")]
        public string firstName { get; set; }

        [BsonElement("lastName")]
        [JsonPropertyName("lastName")]
        public string lastName { get; set; }

        [BsonElement("role")]
        [JsonPropertyName("role")]
        public string role { get; set; }

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string email { get; set; }

        [BsonElement("password")]
        [JsonPropertyName("password")]
        public string password { get; set; }

        [BsonElement("active")]
        [JsonPropertyName("active")]
        public string active  { get; set; }

        [BsonElement("id")]
        [JsonPropertyName("id")]
        public string id { get; set; }

        [BsonElement("medicalRecord")]
        [JsonPropertyName("medicalRecord")]
        public MedicalRecord medicalRecord { get; set; } = null!;



    }
}