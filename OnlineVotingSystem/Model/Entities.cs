namespace OnlineVotingSystem.Model;

public sealed record User(int Id, string Username, string FullName, string Role);
public sealed record Election(int Id, string Title, string Description, DateTime StartDate, DateTime EndDate, bool IsActive);
public sealed record Candidate(int Id, int ElectionId, string Name, string Party, string Description);
public sealed record ResultRow(string CandidateName, string Party, int VoteCount);
