using System.Drawing;
using System.Windows.Forms;
using DriveQuest.Data;
using DriveQuest.Models;

namespace DriveQuest.Forms;

public class RentalsForm : Form
{
    private readonly ComboBox customer = Ui.Combo(300);
    private readonly ComboBox car = Ui.Combo(300);
    private readonly NumericUpDown days = new()
    {
        Minimum = 1, Maximum = 365, Value = 1, Width = 120,
        BackColor = Ui.Surface2, ForeColor = Ui.Text, Font = new Font("Segoe UI", 11)
    };
    private readonly Label summary = Ui.Label("", 13);

    public RentalsForm()
    {
        Ui.StyleForm(this, "Drive Quest | Rent a Car", 850, 620);
        Panel content = new() { Dock = DockStyle.Fill, Padding = new Padding(35), BackColor = Ui.Background };
        Controls.Add(content);
        Label title = Ui.Label("🔑 NEW RENTAL", 23, true, Ui.Gold);
        title.Location = new Point(35, 28); content.Controls.Add(title);
        AddField(content, "Customer", customer, 35, 100);
        AddField(content, "Available Car", car, 365, 100);
        AddField(content, "Number of Days", days, 35, 185);
        Panel summaryCard = new() { Location = new Point(35, 270), Size = new Size(730, 180), BackColor = Ui.Surface, Padding = new Padding(20) };
        content.Controls.Add(summaryCard);
        summary.Location = new Point(20, 17);
        summaryCard.Controls.Add(summary);
        Button confirm = Ui.Button("🚗 CONFIRM RENTAL", Ui.Green);
        confirm.Location = new Point(35, 480); confirm.Width = 230;
        confirm.Click += (_, _) => Confirm();
        content.Controls.Add(confirm);

        customer.SelectedIndexChanged += (_, _) => UpdateSummary();
        car.SelectedIndexChanged += (_, _) => UpdateSummary();
        days.ValueChanged += (_, _) => UpdateSummary();
        Activated += (_, _) => LoadChoices();
        LoadChoices();
    }

    private static void AddField(Control parent, string caption, Control input, int x, int y)
    {
        Label label = Ui.Label(caption, 11, true, Ui.Muted);
        label.Location = new Point(x, y); parent.Controls.Add(label);
        input.Location = new Point(x, y + 30); parent.Controls.Add(input);
    }

    private void LoadChoices()
    {
        int? oldCustomer = (customer.SelectedItem as Customer)?.CustomerID;
        int? oldCar = (car.SelectedItem as Car)?.CarID;
        customer.DataSource = null;
        car.DataSource = null;
        customer.DataSource = DataStore.Customers.ToList();
        car.DataSource = DataStore.Cars.Where(c => c.Status == "Available").ToList();
        if (oldCustomer.HasValue)
        {
            int index = DataStore.Customers.FindIndex(c => c.CustomerID == oldCustomer);
            if (index >= 0) customer.SelectedIndex = index;
        }
        if (oldCar.HasValue)
        {
            int index = ((List<Car>)car.DataSource).FindIndex(c => c.CarID == oldCar);
            if (index >= 0) car.SelectedIndex = index;
        }
        UpdateSummary();
    }

    private void UpdateSummary()
    {
        Customer? selectedCustomer = customer.SelectedItem as Customer;
        Car? selectedCar = car.SelectedItem as Car;
        decimal price = selectedCar?.PricePerDay ?? 0;
        summary.Text =
            $"CUSTOMER:  {selectedCustomer?.FullName ?? "Choose a customer"}\n" +
            $"CAR:  {(selectedCar == null ? "No available car" : selectedCar.Brand + " " + selectedCar.Model)}\n" +
            $"PRICE PER DAY:  ₱{price:N2}\n" +
            $"NUMBER OF DAYS:  {days.Value}\n" +
            $"TOTAL AMOUNT:  ₱{price * days.Value:N2}";
    }

    private void Confirm()
    {
        try
        {
            if (customer.SelectedItem is not Customer selectedCustomer) throw new ArgumentException("Choose a customer.");
            if (car.SelectedItem is not Car selectedCar) throw new ArgumentException("Choose an available car.");
            int before = DataStore.XP;
            Rental rental = DataStore.CreateRental(selectedCustomer.CustomerID, selectedCar.CarID, (int)days.Value);
            MessageBox.Show(
                $"🎉 RENTAL SUCCESSFUL!\n\n{selectedCar.Brand} {selectedCar.Model}\n{rental.NumberOfDays} days\nTotal: ₱{rental.TotalAmount:N2}\n\n⭐ +{DataStore.XP - before} XP",
                "Drive Quest", MessageBoxButtons.OK, MessageBoxIcon.Information);
            days.Value = 1;
            LoadChoices();
        }
        catch (Exception ex) { Ui.Error(ex); }
    }
}
