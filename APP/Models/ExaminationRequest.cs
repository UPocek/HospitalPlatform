using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class ExaminationRequest
{
    [BsonElement("examination")]
    [JsonPropertyName("examination")]
    public Examination examination {get; set;}

    [BsonElement("status")]
    [JsonPropertyName("status")]
    public int status {get; set;}
}