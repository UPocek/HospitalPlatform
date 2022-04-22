using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Examination
{
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id {get; set;}

    [BsonElement("id")]
    [JsonPropertyName("id")]
    public int id {get; set;}


    [BsonElement("done")]
    [JsonPropertyName("done")]
    public bool isExaminationOver {get; set;}

    [BsonElement("date")]
    [JsonPropertyName("date")]
    public string dateAndTimeOfExamination {get; set;}

    [BsonElement("duration")]
    [JsonPropertyName("duration")]
    public int durationOfExamination {get; set;}

    [BsonElement("patient")]
    [JsonPropertyName("patient")]
    public int patinetId {get; set;}

    [BsonElement("doctor")]
    [JsonPropertyName("doctor")]
    public int doctorId {get; set;}

    [BsonElement("room")]
    [JsonPropertyName("room")]
    public string roomName {get; set;}

    [BsonElement("anamnesis")]
    [JsonPropertyName("anamnesis")]
    public string? anamnesis {get; set;} = "";

    [BsonElement("urgent")]
    [JsonPropertyName("urgent")]
    public bool isUrgent {get; set;}

    [BsonElement("type")]
    [JsonPropertyName("type")]
    public string typeOfExamination {get; set;}

    [BsonElement("equipmentUsed")]
    [JsonPropertyName("equipmentUsed")]
    public List<string>? equipmentUsed {get; set;} = new List<string>();

}

