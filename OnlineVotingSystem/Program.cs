namespace OnlineVotingSystem;

using OnlineVotingSystem.Controller;
using OnlineVotingSystem.Database;
using OnlineVotingSystem.View;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        try
        {
            new DatabaseInitializer().Initialize();
            Application.Run(new LoginForm(new ApplicationController()));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to initialize the database.\n\n{ex.Message}", "Startup error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }    
}