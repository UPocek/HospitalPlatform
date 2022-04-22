using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Prescription
{
    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string drugName { get; set; }

    [BsonElement("when")]
    [JsonPropertyName("when")]
    public List<string> whenPrescribed { get; set; }
}
