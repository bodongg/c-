using System.Drawing;
using System.Windows.Forms;
using DriveQuest.Data;

namespace DriveQuest.Forms;

public class ReturnCarForm : Form
{
    private readonly DataGridView grid = Ui.Grid();
    private readonly ComboBox filter = Ui.Combo(160);
    private readonly Button returnButton = Ui.Button("↩ RETURN CAR", Ui.Green);
    private int selectedId;

    public ReturnCarForm(bool showHistory = false)
    {
        Ui.StyleForm(this, showHistory ? "Drive Quest | Rental History" : "Drive Quest | Return Car");
        Panel top = new() { Dock = DockStyle.Top, Height = 125, BackColor = Ui.Surface, Padding = new Padding(22) };
        Controls.Add(grid); Controls.Add(top);
        Label title = Ui.Label(showHistory ? "📋 RENTAL HISTORY" : "↩ RETURN A CAR", 20, true, Ui.Gold);
        title.Location = new Point(22, 15); top.Controls.Add(title);
        Label filterLabel = Ui.Label("Show", 10, true, Ui.Muted);
        filterLabel.Location = new Point(22, 83); top.Controls.Add(filterLabel);
        filter.Items.AddRange(new object[] { "Active", "Completed", "All" });
        filter.SelectedIndex = showHistory ? 2 : 0;
        filter.Location = new Point(80, 78); top.Controls.Add(filter);
        returnButton.Location = new Point(270, 75);
        returnButton.Width = 190;
        top.Controls.Add(returnButton);
        returnButton.Click += (_, _) => ReturnSelected();
        grid.SelectionChanged += (_, _) => SelectRental();
        filter.SelectedIndexChanged += (_, _) => RefreshGrid();
        Activated += (_, _) => RefreshGrid();
        RefreshGrid();
    }

    private void RefreshGrid()
    {
        string status = filter.Text;
        grid.DataSource = DataStore.Rentals
            .Where(r => status == "All" || r.Status == status)
            .Select(r => new
            {
                RentalID = r.RentalID,
                Customer = DataStore.Customers.FirstOrDefault(c => c.CustomerID == r.CustomerID)?.FullName ?? "Unknown",
                Car = DataStore.Cars.Where(c => c.CarID == r.CarID).Select(c => c.Brand + " " + c.Model).FirstOrDefault() ?? "Unknown",
                RentalDate = r.RentalDate.ToString("yyyy-MM-dd HH:mm"),
                ReturnDate = r.ReturnDate?.ToString("yyyy-MM-dd HH:mm") ?? "—",
                Days = r.NumberOfDays,
                Total = r.TotalAmount,
                r.Status
            })
            .ToList();
        if (grid.Columns["RentalID"] != null) grid.Columns["RentalID"].HeaderText = "Rental ID";
        if (grid.Columns["RentalDate"] != null) grid.Columns["RentalDate"].HeaderText = "Rental Date";
        if (grid.Columns["ReturnDate"] != null) grid.Columns["ReturnDate"].HeaderText = "Return Date";
        if (grid.Columns["Total"] != null) grid.Columns["Total"].DefaultCellStyle.Format = "₱#,##0.00";
        grid.ClearSelection(); selectedId = 0; returnButton.Enabled = false;
    }

    private void SelectRental()
    {
        if (grid.SelectedRows.Count == 0 || grid.SelectedRows[0].Cells["RentalID"].Value is not int id)
        {
            selectedId = 0; returnButton.Enabled = false; return;
        }
        selectedId = id;
        returnButton.Enabled = DataStore.Rentals.Any(r => r.RentalID == id && r.Status == "Active");
    }

    private void ReturnSelected()
    {
        try
        {
            if (selectedId == 0) throw new ArgumentException("Select an active rental.");
            var rental = DataStore.Rentals.First(r => r.RentalID == selectedId);
            var car = DataStore.Cars.First(c => c.CarID == rental.CarID);
            DataStore.ReturnCar(selectedId);
            MessageBox.Show($"🎉 CAR RETURNED!\n\n{car.Brand} {car.Model} is now available.\n\n⭐ +25 XP",
                "Drive Quest", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshGrid();
        }
        catch (Exception ex) { Ui.Error(ex); }
    }
}
