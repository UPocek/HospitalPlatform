using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Referral
{
    [BsonElement("refferalId")]
    [JsonPropertyName("refferalId")]
    public int? refferalId {get; set;} = null;

    [BsonElement("doctorId")]
    [JsonPropertyName("doctorId")]
    public int? doctorId {get; set;} = null;

    [BsonElement("speciality")]
    [JsonPropertyName("speciality")]
    public string? speciality {get; set;} = null;

}