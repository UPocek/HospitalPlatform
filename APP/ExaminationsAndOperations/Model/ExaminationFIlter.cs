using System.Text.Json.Serialization;

public class ExaminationFilter
{

    [JsonPropertyName("dueDate")]
    public string dueDate { get; set; }

    [JsonPropertyName("doctor")]
    public int doctorId {get; set;}

    
    [JsonPropertyName("patient")]
    public int patientId {get; set;}
    
    [JsonPropertyName("timeFrom")]
    public string timeFrom {get; set;}

    [JsonPropertyName("timeTo")]
    public string timeTo {get; set;}


    [JsonPropertyName("priority")]
    public string priority {get; set;}
}