using OnlineVotingSystem.Data;
using OnlineVotingSystem.Model;

namespace OnlineVotingSystem.Controller;

public sealed class VotingController
{
    private readonly VotingRepository _repository;
    public VotingController(VotingRepository repository) => _repository = repository;
    public IReadOnlyList<Election> GetElections() => _repository.GetElections();
    public IReadOnlyList<Candidate> GetCandidates(int electionId) => _repository.GetCandidates(electionId);
    public IReadOnlyList<ResultRow> GetResults(int electionId) => _repository.GetResults(electionId);
    public bool HasVoted(int electionId, int userId) => _repository.HasVoted(electionId, userId);
    public void CastVote(int electionId, int candidateId, int userId)
    {
        if (HasVoted(electionId, userId)) throw new InvalidOperationException("You have already voted in this election.");
        _repository.CastVote(electionId, candidateId, userId);
    }
}
