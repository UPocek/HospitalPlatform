using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

[BsonIgnoreExtraElements]
public class Poll
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _Id { get; set; }


    //if hospital is scored - 0, else doctors id
    [BsonElement("who")]
    [JsonPropertyName("who")]
    public int Who { get; set; }

    [BsonElement("score")]
    [JsonPropertyName("score")]
    public int Score { get; set; }

    [BsonElement("recommendation")]
    [JsonPropertyName("recommendation")]
    public int Recommendation { get; set; }

    [BsonElement("comment")]
    [JsonPropertyName("comment")]
    public string Comment { get; set; }
}