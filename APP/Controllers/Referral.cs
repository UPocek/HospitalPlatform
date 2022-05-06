using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Referral
{
    [BsonElement("doctorId")]
    [JsonPropertyName("doctorId")]
    public int? doctorId { get; set; }

    [BsonElement("speciality")]
    [JsonPropertyName("speciality")]
    public string? speciality { get; set; }

}