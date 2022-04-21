using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Models
{
    [BsonIgnoreExtraElements]
    public class MedicalCard
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id {get; set;}

        [BsonElement("id")]
        [JsonPropertyName("id")]
        public int id {get; set;}

        [BsonElement("firstName")]
        [JsonPropertyName("firstName")]
        public string firstName {get; set;}

        [BsonElement("lastName")]
        [JsonPropertyName("lastName")]
        public string lastName {get; set;}

        [BsonElement("medicalRecord")]
        [JsonPropertyName("medicalRecord")]
        public MedicalRecord medicalRecord {get; set;}
    }
}