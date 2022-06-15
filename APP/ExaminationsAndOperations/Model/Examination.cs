using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Examination
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _id { get; set; }

    [BsonElement("id")]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [BsonElement("done")]
    [JsonPropertyName("done")]
    public bool IsExaminationOver { get; set; }

    [BsonElement("date")]
    [JsonPropertyName("date")]
    public string DateAndTimeOfExamination { get; set; }

    [BsonElement("duration")]
    [JsonPropertyName("duration")]
    public int DurationOfExamination { get; set; }

    [BsonElement("patient")]
    [JsonPropertyName("patient")]
    public int PatinetId { get; set; }

    [BsonElement("doctor")]
    [JsonPropertyName("doctor")]
    public int DoctorId { get; set; }

    [BsonElement("room")]
    [JsonPropertyName("room")]
    public string RoomName { get; set; }

    [BsonElement("anamnesis")]
    [JsonPropertyName("anamnesis")]
    public string? Anamnesis {get; set;} = "";

    [BsonElement("urgent")]
    [JsonPropertyName("urgent")]
    public bool IsUrgent { get; set; }

    [BsonElement("type")]
    [JsonPropertyName("type")]
    public string TypeOfExamination { get; set; }

    [BsonElement("equipmentUsed")]
    [JsonPropertyName("equipmentUsed")]

    public List<string>? EquipmentUsed {get; set;} = new List<string>();
}

