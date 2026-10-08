using System.Drawing;
using System.Windows.Forms;

namespace DriveQuest.Forms;

public class LoginForm : Form
{
    private readonly TextBox username = Ui.Input(280);
    private readonly TextBox password = Ui.Input(280);

    public LoginForm()
    {
        Ui.StyleForm(this, "Drive Quest | Login", 550, 550);
        MinimumSize = Size;
        MaximumSize = Size;

        Panel card = new() { BackColor = Ui.Surface, Size = new Size(390, 390), Location = new Point(75, 60), Padding = new Padding(36) };
        Controls.Add(card);
        Label brand = Ui.Label("🚗 DRIVE QUEST", 24, true, Ui.Gold);
        brand.Location = new Point(36, 32);
        card.Controls.Add(brand);
        Label subtitle = Ui.Label("GAMIFIED CAR RENTAL SYSTEM", 10, false, Ui.Muted);
        subtitle.Location = new Point(38, 78);
        card.Controls.Add(subtitle);

        Label userLabel = Ui.Label("Username", 10, true);
        userLabel.Location = new Point(38, 133);
        card.Controls.Add(userLabel);
        username.Location = new Point(38, 160);
        card.Controls.Add(username);
        Label passLabel = Ui.Label("Password", 10, true);
        passLabel.Location = new Point(38, 209);
        card.Controls.Add(passLabel);
        password.Location = new Point(38, 236);
        password.UseSystemPasswordChar = true;
        card.Controls.Add(password);

        Button login = Ui.Button("LOGIN", Ui.Blue);
        login.Location = new Point(38, 300);
        login.Width = 280;
        login.Click += (_, _) => Login();
        card.Controls.Add(login);
        AcceptButton = login;
    }

    private void Login()
    {
        if (username.Text == "admin" && password.Text == "admin123")
        {
            Hide();
            using DashboardForm dashboard = new();
            dashboard.ShowDialog();
            password.Clear();
            Show();
            username.Focus();
        }
        else
        {
            MessageBox.Show("Invalid username or password.", "Drive Quest", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            password.Clear();
            password.Focus();
        }
    }
}
