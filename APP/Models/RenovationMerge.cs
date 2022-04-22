using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class RenovationMerge
{
    public RenovationMerge(string room1, string room2, string startDate, string endDate)
    {
        this.room1 = room1;
        this.room2 = room2;
        this.startDate = startDate;
        this.endDate = endDate;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    [BsonElement("room1")]
    [JsonPropertyName("room1")]
    public string room1 { get; set; }

    [BsonElement("room2")]
    [JsonPropertyName("room2")]
    public string room2 { get; set; }

    [BsonElement("startDate")]
    [JsonPropertyName("startDate")]
    public string startDate { get; set; }

    [BsonElement("endDate")]
    [JsonPropertyName("endDate")]
    public string endDate { get; set; }
}


