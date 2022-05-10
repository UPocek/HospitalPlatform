using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Prescription
{
    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string DrugName { get; set; }

    [BsonElement("when")]
    [JsonPropertyName("when")]
    public string WhenPrescribed { get; set; }

    [BsonElement("how")]
    [JsonPropertyName("how")]
    public string HowPrescribed { get; set; }

    [BsonElement("frequency")]
    [JsonPropertyName("frequency")]
    public int Frequency { get; set; }
}
