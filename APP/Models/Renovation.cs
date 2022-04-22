using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Renovation
{
    public Renovation(string room, string startDate, string endDate)
    {
        this.room = room;
        this.startDate = startDate;
        this.endDate = endDate;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    [BsonElement("room")]
    [JsonPropertyName("room")]
    public string room { get; set; }

    [BsonElement("startDate")]
    [JsonPropertyName("startDate")]
    public string startDate { get; set; }

    [BsonElement("endDate")]
    [JsonPropertyName("endDate")]
    public string endDate { get; set; }
}


