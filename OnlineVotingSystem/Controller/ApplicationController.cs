using OnlineVotingSystem.Data;
using OnlineVotingSystem.Model;

namespace OnlineVotingSystem.Controller;

public sealed class ApplicationController
{
    public VotingRepository Repository { get; } = new();
    public AuthenticationController Authentication { get; }
    public VotingController Voting { get; }
    public User? CurrentUser { get; private set; }

    public ApplicationController()
    {
        Authentication = new AuthenticationController(Repository);
        Voting = new VotingController(Repository);
    }

    public bool SignIn(string username, string password)
    {
        CurrentUser = Authentication.Login(username, password);
        return CurrentUser is not null;
    }

    public void SignOut() => CurrentUser = null;
}
