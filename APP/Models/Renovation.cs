using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Renovation
{
    public Renovation(string room, string startDate, string endDate, bool done, string kind)
    {
        this.room = room;
        this.startDate = startDate;
        this.endDate = endDate;
        this.done = done;
        this.kind = kind;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    [BsonElement("room")]
    [JsonPropertyName("room")]
    public string room { get; set; }

    [BsonElement("room2")]
    [JsonPropertyName("room2")]
    public string? room2 { get; set; }

    [BsonElement("startDate")]
    [JsonPropertyName("startDate")]
    public string startDate { get; set; }

    [BsonElement("endDate")]
    [JsonPropertyName("endDate")]
    public string endDate { get; set; }

    [BsonElement("done")]
    [JsonPropertyName("done")]
    public bool done { get; set; }

    [BsonElement("kind")]
    [JsonPropertyName("kind")]
    public string kind { get; set; }
}


