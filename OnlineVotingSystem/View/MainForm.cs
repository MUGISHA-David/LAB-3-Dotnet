using OnlineVotingSystem.Controller;
using OnlineVotingSystem.Model;

namespace OnlineVotingSystem.View;

public sealed class MainForm : Form
{
    private readonly ApplicationController _controller;
    private readonly ComboBox _elections = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 380 };
    private readonly ListBox _candidates = new() { Width = 520, Height = 180 };
    private readonly DataGridView _results = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, AllowUserToAddRows = false };
    private readonly Label _welcome = new();
    private List<Election> _electionItems = [];
    private List<Candidate> _candidateItems = [];

    public MainForm(ApplicationController controller)
    {
        _controller = controller; Text = "Online Voting System"; Width = 780; Height = 600; StartPosition = FormStartPosition.CenterParent;
        BuildLayout(); LoadElections();
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(15), RowCount = 5, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 35)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 45)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 230)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 45)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _welcome.Text = $"Signed in as: {_controller.CurrentUser?.FullName} ({_controller.CurrentUser?.Role})"; _welcome.AutoSize = true; root.Controls.Add(_welcome, 0, 0);
        var electionPanel = new FlowLayoutPanel { Dock = DockStyle.Fill }; electionPanel.Controls.Add(new Label { Text = "Election:", AutoSize = true, Padding = new Padding(0, 7, 0, 0) }); electionPanel.Controls.Add(_elections); _elections.SelectedIndexChanged += ElectionChanged; root.Controls.Add(electionPanel, 0, 1);
        var candidatesPanel = new GroupBox { Text = "Candidates", Dock = DockStyle.Fill }; candidatesPanel.Controls.Add(_candidates); root.Controls.Add(candidatesPanel, 0, 2);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill }; var vote = new Button { Text = "Cast vote", AutoSize = true }; vote.Click += CastVote; var refresh = new Button { Text = "Refresh results", AutoSize = true }; refresh.Click += (_, _) => LoadResults(); var close = new Button { Text = "Sign out", AutoSize = true }; close.Click += (_, _) => Close(); actions.Controls.Add(vote); actions.Controls.Add(refresh); actions.Controls.Add(close); root.Controls.Add(actions, 0, 3);
        var results = new GroupBox { Text = "Election results", Dock = DockStyle.Fill }; results.Controls.Add(_results); root.Controls.Add(results, 0, 4); Controls.Add(root);
    }

    private void LoadElections()
    {
        _electionItems = _controller.Voting.GetElections().ToList(); _elections.DataSource = _electionItems; _elections.DisplayMember = "Title"; _elections.ValueMember = "Id";
        if (_electionItems.Count > 0) { _elections.SelectedIndex = 0; LoadCandidates(); LoadResults(); }
    }

    private int SelectedElectionId => _elections.SelectedItem is Election election ? election.Id : 0;
    private void ElectionChanged(object? sender, EventArgs e) { LoadCandidates(); LoadResults(); }
    private void LoadCandidates() { if (SelectedElectionId == 0) return; _candidateItems = _controller.Voting.GetCandidates(SelectedElectionId).ToList(); _candidates.DataSource = _candidateItems; _candidates.DisplayMember = "Name"; }
    private void LoadResults() { if (SelectedElectionId == 0) return; _results.DataSource = _controller.Voting.GetResults(SelectedElectionId).Select(r => new { Candidate = r.CandidateName, Party = r.Party, Votes = r.VoteCount }).ToList(); }

    private void CastVote(object? sender, EventArgs e)
    {
        if (_candidates.SelectedItem is not Candidate candidate || _controller.CurrentUser is null) { MessageBox.Show(this, "Select a candidate first."); return; }
        if (MessageBox.Show(this, $"Cast your vote for {candidate.Name}? This cannot be undone.", "Confirm vote", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try { _controller.Voting.CastVote(SelectedElectionId, candidate.Id, _controller.CurrentUser.Id); MessageBox.Show(this, "Your vote was recorded.", "Success"); LoadResults(); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Vote not recorded", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }
}
