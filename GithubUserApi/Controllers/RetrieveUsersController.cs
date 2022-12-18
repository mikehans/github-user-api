using GithubUserApi.DTOs;
using GithubUserApi.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GithubUserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RetrieveUsersController : ControllerBase
{
    private readonly GithubOptions _options;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IGitUserService _service;
    private readonly ILogger<RetrieveUsersController> _logger;

    public RetrieveUsersController(IOptions<GithubOptions> options, IHttpClientFactory clientFactory, IGitUserService service, ILogger<RetrieveUsersController> logger)
    {
        _options = options.Value;
        _clientFactory = clientFactory;
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GithubUser>>> GetUsers([FromQuery] List<string> userNames)
    {
        var distinctUserNames = userNames.Distinct().ToList();
        IEnumerable<GithubUser> githubUsers = await _service.ProcessList(distinctUserNames);

        return Ok(githubUsers);
    }
}
