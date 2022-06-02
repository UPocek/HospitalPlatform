using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Referral
{
    [BsonElement("doctorId")]
    [JsonPropertyName("doctorId")]
    public int? DoctorId {get; set;} = null;

    [BsonElement("speciality")]
    [JsonPropertyName("speciality")]
    public string? Speciality {get; set;} = null;

    [BsonElement("referralId")]
    [JsonPropertyName("referralId")]
    public int? ReferralId {get; set;} = null;

}