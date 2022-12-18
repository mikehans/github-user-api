using System.Text.Json.Serialization;

namespace GithubUserApi.DTOs;

public class GithubUser
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;
    [JsonPropertyName("company")]
    public string Company { get; set; } = string.Empty;
    [JsonPropertyName("followers")]
    public int NumberOfFollowers { get; set; }
    [JsonPropertyName("public_repos")]
    public int NumberOfPublicRepos { get; set; }
    public double AverageFollowersPerRepo { get; set; }
}
