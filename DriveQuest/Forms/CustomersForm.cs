using System.Drawing;
using System.Windows.Forms;
using DriveQuest.Data;

namespace DriveQuest.Forms;

public class CustomersForm : Form
{
    private readonly DataGridView grid = Ui.Grid();
    private readonly TextBox name = Ui.Input(220), phone = Ui.Input(170), license = Ui.Input(200), search = Ui.Input(220);
    private int selectedId;

    public CustomersForm()
    {
        Ui.StyleForm(this, "Drive Quest | Customers");
        Panel top = new() { Dock = DockStyle.Top, Height = 205, Padding = new Padding(22), BackColor = Ui.Surface };
        Controls.Add(grid); Controls.Add(top);
        Label title = Ui.Label("👥 CUSTOMER MANAGEMENT", 20, true, Ui.Gold);
        title.Location = new Point(22, 14); top.Controls.Add(title);
        AddField(top, "Full Name", name, 22);
        AddField(top, "Phone", phone, 265);
        AddField(top, "License Number", license, 457);
        Button add = Ui.Button("ADD", Ui.Green), update = Ui.Button("UPDATE"), delete = Ui.Button("DELETE", Ui.Red);
        Button clear = Ui.Button("CLEAR", Ui.Surface2), find = Ui.Button("SEARCH");
        Place(top, add, 22); Place(top, update, 166); Place(top, delete, 310); Place(top, clear, 454);
        search.Location = new Point(620, 150); top.Controls.Add(search); Place(top, find, 855);
        add.Click += (_, _) => Run(() =>
        {
            int before = DataStore.XP;
            DataStore.AddCustomer(name.Text, phone.Text, license.Text);
            RefreshGrid(); ClearFields();
            MessageBox.Show($"Customer added! ⭐ +{DataStore.XP - before} XP", "Drive Quest");
        });
        update.Click += (_, _) => Run(() =>
        {
            DataStore.UpdateCustomer(selectedId, name.Text, phone.Text, license.Text);
            RefreshGrid(); ClearFields();
        });
        delete.Click += (_, _) => Run(() =>
        {
            DataStore.DeleteCustomer(selectedId); RefreshGrid(); ClearFields();
        });
        clear.Click += (_, _) => { search.Clear(); ClearFields(); RefreshGrid(); };
        find.Click += (_, _) => RefreshGrid();
        search.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { RefreshGrid(); e.SuppressKeyPress = true; } };
        grid.SelectionChanged += (_, _) => LoadSelection();
        RefreshGrid();
    }

    private static void AddField(Control parent, string caption, Control input, int x)
    {
        Label label = Ui.Label(caption, 10, true, Ui.Muted);
        label.Location = new Point(x, 65); parent.Controls.Add(label);
        input.Location = new Point(x, 93); parent.Controls.Add(input);
    }

    private static void Place(Control parent, Button button, int x)
    {
        button.Location = new Point(x, 148); parent.Controls.Add(button);
    }

    private void RefreshGrid()
    {
        string term = search.Text.Trim();
        grid.DataSource = DataStore.Customers
            .Where(c => c.FullName.Contains(term, StringComparison.OrdinalIgnoreCase) || c.Phone.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(c => new { ID = c.CustomerID, Name = c.FullName, c.Phone, LicenseNumber = c.LicenseNumber })
            .ToList();
        if (grid.Columns["LicenseNumber"] != null) grid.Columns["LicenseNumber"].HeaderText = "License Number";
        grid.ClearSelection(); selectedId = 0;
    }

    private void LoadSelection()
    {
        if (grid.SelectedRows.Count == 0 || grid.SelectedRows[0].Cells["ID"].Value is not int id) return;
        var customer = DataStore.Customers.FirstOrDefault(c => c.CustomerID == id);
        if (customer == null) return;
        selectedId = id; name.Text = customer.FullName; phone.Text = customer.Phone; license.Text = customer.LicenseNumber;
    }

    private void ClearFields()
    {
        selectedId = 0; name.Clear(); phone.Clear(); license.Clear(); grid.ClearSelection();
    }

    private static void Run(Action action)
    {
        try { action(); }
        catch (Exception ex) { Ui.Error(ex); }
    }
}
