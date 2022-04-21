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

        [BsonElement("wieght")]
        [JsonPropertyName("weight")]
        public int weight {get; set;}

        [BsonElement("bloodType")]
        [JsonPropertyName("bloodType")]
        public string bloodType {get; set;}

        [BsonElement("diseases")]
        [JsonPropertyName("diseases")]
        public List<string> diseases {get; set;}

        [BsonElement("alergies")]
        [JsonPropertyName("alergies")]
        public List<string> alergies {get; set;}

        [BsonElement("drugs")]
        [JsonPropertyName("drugs")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> _drugs {get; set;}

        [BsonElement("examinations")]
        [JsonPropertyName("examinations")]
        public List<int> _examinations {get; set;}

        public List<Examination> patientsExaminations {get; set;} = null;

        public List<string> patientsDrugs {get; set;} = null;

    }
}