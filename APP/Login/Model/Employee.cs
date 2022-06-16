using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

[BsonIgnoreExtraElements]
public class Employee
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _Id { get; set; }

    [BsonElement("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [BsonElement("firstName")]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [BsonElement("lastName")]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [BsonElement("email")]
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [BsonElement("role")]
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [BsonElement("specialization")]
    [JsonPropertyName("specialization")]
    public string? Specialization { get; set; }

    [BsonElement("freeDays")]
    [JsonPropertyName("freeDays")]
    public List<FreeDay>? FreeDays {get; set;}
}