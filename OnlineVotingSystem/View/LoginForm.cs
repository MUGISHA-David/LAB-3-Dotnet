using OnlineVotingSystem.Controller;

namespace OnlineVotingSystem.View;

public sealed class LoginForm : Form
{
    private readonly ApplicationController _controller;
    private readonly TextBox _username = new();
    private readonly TextBox _password = new();
    private readonly Label _status = new();

    public LoginForm(ApplicationController controller)
    {
        _controller = controller;
        Text = "Online Voting System - Sign in"; Width = 430; Height = 300; StartPosition = FormStartPosition.CenterScreen;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(30), ColumnCount = 2, RowCount = 5 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.Controls.Add(new Label { Text = "Username", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0); layout.Controls.Add(_username, 1, 0);
        layout.Controls.Add(new Label { Text = "Password", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1); layout.Controls.Add(_password, 1, 1); _password.UseSystemPasswordChar = true;
        var login = new Button { Text = "Sign in", AutoSize = true }; login.Click += Login;
        layout.Controls.Add(login, 1, 2); _status.Text = "Demo accounts: admin/admin123 or voter/voter123"; _status.AutoSize = true; layout.Controls.Add(_status, 0, 4); layout.SetColumnSpan(_status, 2);
        Controls.Add(layout); AcceptButton = login;
    }

    private void Login(object? sender, EventArgs e)
    {
        try
        {
            if (_controller.SignIn(_username.Text, _password.Text)) { Hide(); using var main = new MainForm(_controller); main.ShowDialog(this); _controller.SignOut(); Show(); _password.Clear(); }
            else _status.Text = "Invalid username or password.";
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
