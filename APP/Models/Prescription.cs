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
    public string whenPrescribed { get; set; }

    [BsonElement("how")]
    [JsonPropertyName("how")]
    public string howPrescribed { get; set; }

    [BsonElement("frequency")]
    [JsonPropertyName("frequency")]
    public int frequency { get; set; }
}
