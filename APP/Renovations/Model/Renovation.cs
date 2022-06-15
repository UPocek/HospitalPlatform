using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Renovation
{
    public Renovation(string room, string startDate, string endDate, bool done, string kind)
    {
        this.Room = room;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.Done = done;
        this.Kind = kind;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("room")]
    [JsonPropertyName("room")]
    public string Room { get; set; }

    [BsonElement("room2")]
    [JsonPropertyName("room2")]
    public string? Room2 { get; set; }

    [BsonElement("startDate")]
    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }

    [BsonElement("endDate")]
    [JsonPropertyName("endDate")]
    public string EndDate { get; set; }

    [BsonElement("done")]
    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [BsonElement("kind")]
    [JsonPropertyName("kind")]
    public string Kind { get; set; }
}


