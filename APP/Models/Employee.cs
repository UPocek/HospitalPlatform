using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

[BsonIgnoreExtraElements]
public class Employee
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }

    [BsonElement("firstName")]
    [JsonPropertyName("firstName")]
    public string firstName { get; set; }

    [BsonElement("lastName")]
    [JsonPropertyName("lastName")]
    public string dateAndlastName { get; set; }

    [BsonElement("role")]
    [JsonPropertyName("role")]
    public string role { get; set; }

    [BsonElement("id")]
    [JsonPropertyName("id")]
    public int id { get; set; }


    [BsonElement("specialization")]
    [JsonPropertyName("specialization")]
    public string? specialization { get; set; }
}