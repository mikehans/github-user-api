namespace GithubUserApi;

public class GithubOptions
{
    public const string Github = "Github";

    public string ApiEndpoint { get; set; } = String.Empty;
    public string Token { get; set; } = String.Empty;
    public HttpUserAgent? HttpUserAgent { get; set; }
}

public class HttpUserAgent
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}
