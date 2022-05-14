using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class RenovationMerge
{
    public RenovationMerge(string room1, string room2, string startDate, string endDate)
    {
        this.Room1 = room1;
        this.Room2 = room2;
        this.StartDate = startDate;
        this.EndDate = endDate;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("room1")]
    [JsonPropertyName("room1")]
    public string Room1 { get; set; }

    [BsonElement("room2")]
    [JsonPropertyName("room2")]
    public string Room2 { get; set; }

    [BsonElement("startDate")]
    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }

    [BsonElement("endDate")]
    [JsonPropertyName("endDate")]
    public string EndDate { get; set; }
}


