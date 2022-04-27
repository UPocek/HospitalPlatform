using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

public class Account
{
    public Account()
    {

    }

    public Account(string token, User user)
    {
        this.token = token;
        this.user = user;
    }

    [BsonElement("token")]
    [JsonPropertyName("token")]
    public string token { get; set; }

    [BsonElement("user")]
    [JsonPropertyName("user")]
    public User user { get; set; }
}
