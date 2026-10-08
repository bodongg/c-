using System.Drawing;
using System.Windows.Forms;
using DriveQuest.Data;

namespace DriveQuest.Forms;

public class CarsForm : Form
{
    private readonly DataGridView grid = Ui.Grid();
    private readonly TextBox brand = Ui.Input(150), model = Ui.Input(150), year = Ui.Input(100);
    private readonly TextBox price = Ui.Input(130), search = Ui.Input(220);
    private readonly ComboBox category = Ui.Combo(145);
    private int selectedId;

    public CarsForm()
    {
        Ui.StyleForm(this, "Drive Quest | Cars");
        Panel top = new() { Dock = DockStyle.Top, Height = 205, Padding = new Padding(22), BackColor = Ui.Surface };
        Controls.Add(grid);
        Controls.Add(top);
        Label title = Ui.Label("🚗 FLEET MANAGEMENT", 20, true, Ui.Gold);
        title.Location = new Point(22, 14);
        top.Controls.Add(title);
        AddField(top, "Brand", brand, 22);
        AddField(top, "Model", model, 190);
        AddField(top, "Year", year, 358);
        AddField(top, "Category", category, 476);
        AddField(top, "Price / Day", price, 640);
        category.Items.AddRange(new object[] { "Sedan", "SUV", "MPV", "Hatchback", "Van", "Other" });
        category.SelectedIndex = 0;

        Button add = Ui.Button("ADD", Ui.Green), update = Ui.Button("UPDATE"), delete = Ui.Button("DELETE", Ui.Red);
        Button clear = Ui.Button("CLEAR", Ui.Surface2), find = Ui.Button("SEARCH", Ui.Blue);
        Place(top, add, 22); Place(top, update, 166); Place(top, delete, 310); Place(top, clear, 454);
        search.Location = new Point(620, 150); top.Controls.Add(search);
        Place(top, find, 855);
        add.Click += (_, _) => Run(() =>
        {
            int before = DataStore.XP;
            DataStore.AddCar(brand.Text, model.Text, ParseYear(), category.Text, ParsePrice());
            RefreshGrid(); ClearFields();
            MessageBox.Show($"Car added! ⭐ +{DataStore.XP - before} XP", "Drive Quest");
        });
        update.Click += (_, _) => Run(() =>
        {
            DataStore.UpdateCar(selectedId, brand.Text, model.Text, ParseYear(), category.Text, ParsePrice());
            RefreshGrid(); ClearFields();
        });
        delete.Click += (_, _) => Run(() =>
        {
            DataStore.DeleteCar(selectedId);
            RefreshGrid(); ClearFields();
        });
        clear.Click += (_, _) => { search.Clear(); ClearFields(); RefreshGrid(); };
        find.Click += (_, _) => RefreshGrid();
        search.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { RefreshGrid(); e.SuppressKeyPress = true; } };
        grid.SelectionChanged += (_, _) => LoadSelection();
        grid.CellFormatting += (_, e) =>
        {
            if (grid.Columns[e.ColumnIndex].Name == "Status" && e.Value is string status)
                e.CellStyle.ForeColor = status == "Available" ? Ui.Green : Ui.Red;
        };
        RefreshGrid();
    }

    private static void AddField(Control parent, string name, Control input, int x)
    {
        Label label = Ui.Label(name, 10, true, Ui.Muted);
        label.Location = new Point(x, 65); parent.Controls.Add(label);
        input.Location = new Point(x, 93); parent.Controls.Add(input);
    }

    private static void Place(Control parent, Button button, int x)
    {
        button.Location = new Point(x, 148); parent.Controls.Add(button);
    }

    private int ParseYear() => int.TryParse(year.Text, out int value) ? value : throw new ArgumentException("Enter a valid year.");
    private decimal ParsePrice() => decimal.TryParse(price.Text, out decimal value) ? value : throw new ArgumentException("Enter a valid price per day.");

    private void RefreshGrid()
    {
        string term = search.Text.Trim();
        grid.DataSource = DataStore.Cars
            .Where(c => c.Brand.Contains(term, StringComparison.OrdinalIgnoreCase) || c.Model.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(c => new { ID = c.CarID, c.Brand, c.Model, c.Year, c.Category, PricePerDay = c.PricePerDay, c.Status })
            .ToList();
        if (grid.Columns["PricePerDay"] != null)
        {
            grid.Columns["PricePerDay"].HeaderText = "Price / Day";
            grid.Columns["PricePerDay"].DefaultCellStyle.Format = "₱#,##0.00";
        }
        grid.ClearSelection(); selectedId = 0;
    }

    private void LoadSelection()
    {
        if (grid.SelectedRows.Count == 0 || grid.SelectedRows[0].Cells["ID"].Value is not int id) return;
        var car = DataStore.Cars.FirstOrDefault(c => c.CarID == id);
        if (car == null) return;
        selectedId = id;
        brand.Text = car.Brand; model.Text = car.Model; year.Text = car.Year.ToString();
        category.Text = car.Category; price.Text = car.PricePerDay.ToString("0.00");
    }

    private void ClearFields()
    {
        selectedId = 0; brand.Clear(); model.Clear(); year.Clear(); price.Clear();
        category.SelectedIndex = 0; grid.ClearSelection();
    }

    private static void Run(Action action)
    {
        try { action(); }
        catch (Exception ex) { Ui.Error(ex); }
    }
}
