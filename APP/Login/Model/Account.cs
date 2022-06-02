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
        this.Token = token;
        this.User = user;
    }

    [BsonElement("token")]
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [BsonElement("user")]
    [JsonPropertyName("user")]
    public User User { get; set; }
}
