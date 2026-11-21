using Microsoft.Data.SqlClient;
using OnlineVotingSystem.Data;
using System.Security.Cryptography;
using System.Text;

namespace OnlineVotingSystem.Database;

public sealed class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer()
    {
        _connectionString = DatabaseSettings.ConnectionString;
    }

    public void Initialize()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        var databaseName = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        using (var master = new SqlConnection(builder.ConnectionString))
        {
            master.Open();
            using var command = master.CreateCommand();
            var safeName = databaseName.Replace("]", "]]", StringComparison.Ordinal);
            command.CommandText = $"IF DB_ID(N'{databaseName.Replace("'", "''", StringComparison.Ordinal)}') IS NULL CREATE DATABASE [{safeName}]";
            command.ExecuteNonQuery();
        }

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var schema = connection.CreateCommand();
        schema.CommandText = """
            IF OBJECT_ID('Users') IS NULL
            CREATE TABLE Users (Id INT IDENTITY PRIMARY KEY, Username NVARCHAR(50) NOT NULL UNIQUE, PasswordHash NVARCHAR(128) NOT NULL, FullName NVARCHAR(100) NOT NULL, Role NVARCHAR(20) NOT NULL DEFAULT 'Voter');
            IF OBJECT_ID('Elections') IS NULL
            CREATE TABLE Elections (Id INT IDENTITY PRIMARY KEY, Title NVARCHAR(150) NOT NULL, Description NVARCHAR(500) NOT NULL, StartDate DATETIME2 NOT NULL, EndDate DATETIME2 NOT NULL, IsActive BIT NOT NULL DEFAULT 1);
            IF OBJECT_ID('Candidates') IS NULL
            CREATE TABLE Candidates (Id INT IDENTITY PRIMARY KEY, ElectionId INT NOT NULL REFERENCES Elections(Id) ON DELETE CASCADE, Name NVARCHAR(100) NOT NULL, Party NVARCHAR(100) NOT NULL, Description NVARCHAR(300) NOT NULL);
            IF OBJECT_ID('Votes') IS NULL
            CREATE TABLE Votes (Id INT IDENTITY PRIMARY KEY, ElectionId INT NOT NULL REFERENCES Elections(Id), CandidateId INT NOT NULL REFERENCES Candidates(Id), UserId INT NOT NULL REFERENCES Users(Id), VotedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(), CONSTRAINT UQ_Vote_Election_User UNIQUE(ElectionId, UserId));
            """;
        schema.ExecuteNonQuery();

        using var seed = connection.CreateCommand();
        seed.CommandText = """
            IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
                INSERT Users (Username, PasswordHash, FullName, Role) VALUES ('admin', @AdminHash, 'System Administrator', 'Admin');
            IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'voter')
                INSERT Users (Username, PasswordHash, FullName, Role) VALUES ('voter', @VoterHash, 'Demo Voter', 'Voter');
            IF NOT EXISTS (SELECT 1 FROM Elections)
            BEGIN
                INSERT Elections (Title, Description, StartDate, EndDate, IsActive) VALUES ('Student Council Election', 'Choose your student council representative.', DATEADD(day, -1, SYSUTCDATETIME()), DATEADD(day, 30, SYSUTCDATETIME()), 1);
                DECLARE @ElectionId INT = SCOPE_IDENTITY();
                INSERT Candidates (ElectionId, Name, Party, Description) VALUES (@ElectionId, 'Alex Johnson', 'Forward Together', 'Focused on student activities and inclusion.'), (@ElectionId, 'Jordan Smith', 'Student First', 'Focused on improved student services.');
            END
            """;
        seed.Parameters.AddWithValue("@AdminHash", Hash("admin123"));
        seed.Parameters.AddWithValue("@VoterHash", Hash("voter123"));
        seed.ExecuteNonQuery();
    }

    public static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
