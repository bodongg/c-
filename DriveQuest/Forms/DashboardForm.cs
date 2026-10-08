using System.Drawing;
using System.Windows.Forms;
using DriveQuest.Data;

namespace DriveQuest.Forms;

public class DashboardForm : Form
{
    private readonly Label available = Ui.Label("", 20, true, Ui.Green);
    private readonly Label active = Ui.Label("", 20, true, Ui.Blue);
    private readonly Label customers = Ui.Label("", 20, true);
    private readonly Label revenue = Ui.Label("", 20, true, Ui.Gold);
    private readonly Label level = Ui.Label("", 19, true, Ui.Gold);
    private readonly Label xp = Ui.Label("", 11, false, Ui.Muted);
    private readonly ProgressBar progress = new() { Height = 22, Dock = DockStyle.Top };
    private readonly Label missions = Ui.Label("", 11);

    public DashboardForm()
    {
        Ui.StyleForm(this, "Drive Quest | Dashboard", 1220, 760);
        Panel side = new() { Dock = DockStyle.Left, Width = 235, BackColor = Ui.Surface, Padding = new Padding(18, 26, 18, 18) };
        Controls.Add(side);
        Label logo = Ui.Label("🚗 DRIVE QUEST", 17, true, Ui.Gold);
        logo.Dock = DockStyle.Top;
        logo.Height = 68;
        side.Controls.Add(logo);

        FlowLayoutPanel nav = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
        side.Controls.Add(nav);
        side.Controls.SetChildIndex(logo, 0);
        AddNav(nav, "🏠 Dashboard", () => RefreshDashboard());
        AddNav(nav, "🚗 Cars", () => Open(new CarsForm()));
        AddNav(nav, "👥 Customers", () => Open(new CustomersForm()));
        AddNav(nav, "🔑 Rent a Car", () => Open(new RentalsForm()));
        AddNav(nav, "↩ Return Car", () => Open(new ReturnCarForm()));
        AddNav(nav, "📋 Rental History", () => Open(new ReturnCarForm(true)));
        AddNav(nav, "🚪 Logout", Close);

        Panel main = new() { Dock = DockStyle.Fill, Padding = new Padding(28), AutoScroll = true };
        Controls.Add(main);
        side.BringToFront();
        Label heading = Ui.Label("COMMAND CENTER", 25, true);
        heading.Dock = DockStyle.Top;
        heading.Height = 45;
        main.Controls.Add(heading);
        Label subtitle = Ui.Label("GAMIFIED CAR RENTAL MANAGEMENT SYSTEM", 10, false, Ui.Muted);
        subtitle.Dock = DockStyle.Top;
        subtitle.Height = 46;
        main.Controls.Add(subtitle);
        main.Controls.SetChildIndex(subtitle, 0);

        TableLayoutPanel cards = new() { Dock = DockStyle.Top, Height = 150, ColumnCount = 4, Padding = new Padding(0, 8, 0, 14) };
        for (int i = 0; i < 4; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        cards.Controls.Add(Card("🚗 AVAILABLE CARS", available), 0, 0);
        cards.Controls.Add(Card("🔑 ACTIVE RENTALS", active), 1, 0);
        cards.Controls.Add(Card("👥 CUSTOMERS", customers), 2, 0);
        cards.Controls.Add(Card("💰 TOTAL REVENUE", revenue), 3, 0);
        main.Controls.Add(cards);
        main.Controls.SetChildIndex(cards, 0);

        Panel game = new() { Dock = DockStyle.Top, Height = 120, BackColor = Ui.Surface, Padding = new Padding(20), Margin = new Padding(0, 8, 0, 12) };
        game.Controls.Add(progress);
        game.Controls.Add(xp);
        game.Controls.Add(level);
        level.Dock = DockStyle.Top; level.Height = 32;
        xp.Dock = DockStyle.Top; xp.Height = 28;
        game.Controls.SetChildIndex(level, 0);
        game.Controls.SetChildIndex(xp, 1);
        game.Controls.SetChildIndex(progress, 2);
        main.Controls.Add(game);
        main.Controls.SetChildIndex(game, 0);

        Panel missionPanel = new() { Dock = DockStyle.Top, Height = 190, BackColor = Ui.Surface, Padding = new Padding(20) };
        Label missionTitle = Ui.Label("🎯 MISSIONS", 17, true, Ui.Gold);
        missionTitle.Dock = DockStyle.Top; missionTitle.Height = 36;
        missions.Dock = DockStyle.Fill;
        missionPanel.Controls.Add(missions);
        missionPanel.Controls.Add(missionTitle);
        main.Controls.Add(missionPanel);
        main.Controls.SetChildIndex(heading, 0);
        main.Controls.SetChildIndex(subtitle, 1);
        main.Controls.SetChildIndex(cards, 2);
        main.Controls.SetChildIndex(game, 3);
        main.Controls.SetChildIndex(missionPanel, 4);

        Shown += (_, _) => RefreshDashboard();
        Activated += (_, _) => RefreshDashboard();
    }

    private static Panel Card(string title, Label value)
    {
        Panel card = new() { Dock = DockStyle.Fill, BackColor = Ui.Surface, Margin = new Padding(0, 0, 12, 0), Padding = new Padding(15) };
        Label caption = Ui.Label(title, 10, true, Ui.Muted);
        caption.Dock = DockStyle.Top; caption.Height = 34;
        value.Dock = DockStyle.Top;
        card.Controls.Add(value);
        card.Controls.Add(caption);
        return card;
    }

    private static void AddNav(FlowLayoutPanel nav, string caption, Action action)
    {
        Button button = Ui.Button(caption, Ui.Surface2);
        button.Width = 194; button.Height = 48;
        button.TextAlign = ContentAlignment.MiddleLeft;
        button.Margin = new Padding(0, 0, 0, 8);
        button.Click += (_, _) => action();
        nav.Controls.Add(button);
    }

    private void Open(Form form)
    {
        using (form) form.ShowDialog(this);
        RefreshDashboard();
    }

    private void RefreshDashboard()
    {
        available.Text = DataStore.Cars.Count(c => c.Status == "Available").ToString();
        active.Text = DataStore.Rentals.Count(r => r.Status == "Active").ToString();
        customers.Text = DataStore.Customers.Count.ToString();
        revenue.Text = DataStore.CompletedRevenue.ToString("₱#,##0.00");
        level.Text = $"⭐ LEVEL {DataStore.Level}";
        xp.Text = $"{DataStore.XP} XP" + (DataStore.Level == 5 ? " · MAX LEVEL" : $" · Next level at {DataStore.LevelEnd} XP");
        progress.Minimum = 0;
        progress.Maximum = Math.Max(1, DataStore.LevelEnd - DataStore.LevelStart);
        progress.Value = DataStore.Level == 5 ? progress.Maximum : Math.Min(progress.Maximum, DataStore.XP - DataStore.LevelStart);
        missions.Text =
            $"🎯 RENTAL ROOKIE   {Math.Min(1, DataStore.Rentals.Count)}/1   Reward: +50 XP {(DataStore.MissionClaimed("rental") ? "✓" : "")}\n" +
            $"🎯 FLEET BUILDER   {Math.Min(5, DataStore.Cars.Count)}/5   Reward: +50 XP {(DataStore.MissionClaimed("fleet") ? "✓" : "")}\n" +
            $"🎯 CUSTOMER SERVICE   {Math.Min(3, DataStore.Customers.Count)}/3   Reward: +50 XP {(DataStore.MissionClaimed("customer") ? "✓" : "")}";
    }
}
