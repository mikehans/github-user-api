using GithubUserApi.Controllers;
using GithubUserApi.DTOs;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GithubUserApi.Domain;

public class GitUserService : IGitUserService
{
    private readonly GithubOptions _options;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<GitUserService> _logger;

    public GitUserService(IOptions<GithubOptions> options, IHttpClientFactory clientFactory, ILogger<GitUserService> logger)
    {
        _options = options.Value;
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<GithubUser>> ProcessList(List<string> userNames)
    {
        _logger.LogInformation("Executing ProcessList");
        List<Task<GithubUser>> githubUsersTasks = new List<Task<GithubUser>>();
        userNames.ForEach(username =>
        {
            try
            {
                var returnedUser = GetGithubUser(username);
                githubUsersTasks.Add(returnedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inside ProcessList: {ex.Message}");
                throw;
            }
        });
        _logger.LogInformation("Done ProcessList");

        return await Task.WhenAll<GithubUser>(githubUsersTasks);
    }

    private async Task<GithubUser> GetGithubUser(string username)
    {
        _logger.LogInformation($"Executing GetGithubUser for {username}");
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"users/{username}");
            request.Headers.Add("Accept", "application/vnd.github+json");
            request.Headers.Add("Authorization", $"Bearer {_options.Token}");

            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_options.ApiEndpoint);
            client.DefaultRequestHeaders.Clear();
            var productValue = new ProductInfoHeaderValue(_options.HttpUserAgent.Name, _options.HttpUserAgent.Version);

            client.DefaultRequestHeaders.UserAgent.Add(productValue);
            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("User not found");
                return new GithubUser()
                {
                    Login = username,
                    Name = "NOT FOUND"
                };
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Returned HTTP 401 from Github");
            }
            else
            {
                _logger.LogInformation("Found. Returning user");
                var responseBody = await response.Content.ReadAsStringAsync();
                GithubUser? githubUser = JsonSerializer.Deserialize<GithubUser>(responseBody);
                if (githubUser != null && githubUser.NumberOfFollowers > 0 && githubUser.NumberOfPublicRepos > 0)
                {
                    githubUser.AverageFollowersPerRepo = githubUser.NumberOfFollowers / githubUser.NumberOfPublicRepos;
                }

                return githubUser;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error inside GetGithubUser: {ex.Message}");
            throw;
        }

    }

}
