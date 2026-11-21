using Microsoft.Data.SqlClient;
using OnlineVotingSystem.Database;
using OnlineVotingSystem.Model;

namespace OnlineVotingSystem.Data;

public sealed class VotingRepository
{
    private readonly SqlConnectionFactory _factory = new();

    public User? Authenticate(string username, string password)
    {
        using var connection = _factory.Create();
        connection.Open();
        using var command = new SqlCommand("SELECT Id, Username, FullName, Role FROM Users WHERE Username=@Username AND PasswordHash=@PasswordHash", connection);
        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@PasswordHash", DatabaseInitializer.Hash(password));
        using var reader = command.ExecuteReader();
        return reader.Read() ? new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)) : null;
    }

    public List<Election> GetElections()
    {
        using var connection = _factory.Create(); connection.Open();
        using var command = new SqlCommand("SELECT Id, Title, Description, StartDate, EndDate, IsActive FROM Elections ORDER BY StartDate DESC", connection);
        using var reader = command.ExecuteReader(); var items = new List<Election>();
        while (reader.Read()) items.Add(new Election(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), reader.GetBoolean(5)));
        return items;
    }

    public List<Candidate> GetCandidates(int electionId)
    {
        using var connection = _factory.Create(); connection.Open();
        using var command = new SqlCommand("SELECT Id, ElectionId, Name, Party, Description FROM Candidates WHERE ElectionId=@ElectionId ORDER BY Name", connection);
        command.Parameters.AddWithValue("@ElectionId", electionId); using var reader = command.ExecuteReader(); var items = new List<Candidate>();
        while (reader.Read()) items.Add(new Candidate(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
        return items;
    }

    public bool HasVoted(int electionId, int userId)
    {
        using var connection = _factory.Create(); connection.Open();
        using var command = new SqlCommand("SELECT COUNT(*) FROM Votes WHERE ElectionId=@ElectionId AND UserId=@UserId", connection);
        command.Parameters.AddWithValue("@ElectionId", electionId); command.Parameters.AddWithValue("@UserId", userId);
        return (int)command.ExecuteScalar()! > 0;
    }

    public void CastVote(int electionId, int candidateId, int userId)
    {
        using var connection = _factory.Create(); connection.Open();
        using var command = new SqlCommand("INSERT Votes (ElectionId, CandidateId, UserId) VALUES (@ElectionId, @CandidateId, @UserId)", connection);
        command.Parameters.AddWithValue("@ElectionId", electionId); command.Parameters.AddWithValue("@CandidateId", candidateId); command.Parameters.AddWithValue("@UserId", userId); command.ExecuteNonQuery();
    }

    public List<ResultRow> GetResults(int electionId)
    {
        using var connection = _factory.Create(); connection.Open();
        using var command = new SqlCommand("SELECT c.Name, c.Party, COUNT(v.Id) FROM Candidates c LEFT JOIN Votes v ON c.Id=v.CandidateId WHERE c.ElectionId=@ElectionId GROUP BY c.Name, c.Party ORDER BY COUNT(v.Id) DESC, c.Name", connection);
        command.Parameters.AddWithValue("@ElectionId", electionId); using var reader = command.ExecuteReader(); var items = new List<ResultRow>();
        while (reader.Read()) items.Add(new ResultRow(reader.GetString(0), reader.GetString(1), reader.GetInt32(2)));
        return items;
    }
}
