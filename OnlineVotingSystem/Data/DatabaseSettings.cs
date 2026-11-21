namespace OnlineVotingSystem.Data;

public static class DatabaseSettings
{
    public static string ConnectionString { get; set; } =
        "Server=(localdb)\\MSSQLLocalDB;Database=OnlineVotingSystem;Trusted_Connection=True;TrustServerCertificate=True;";
}
