using GithubUserApi.DTOs;

namespace GithubUserApi.Domain;

public interface IGitUserService
{
    public Task<IEnumerable<GithubUser>> ProcessList(List<string> userNames);
}
