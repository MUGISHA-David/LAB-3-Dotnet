using OnlineVotingSystem.Data;
using OnlineVotingSystem.Model;

namespace OnlineVotingSystem.Controller;

public sealed class AuthenticationController
{
    private readonly VotingRepository _repository;
    public AuthenticationController(VotingRepository repository) => _repository = repository;
    public User? Login(string username, string password) => _repository.Authenticate(username.Trim(), password);
}
