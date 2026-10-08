using DriveQuest.Models;

namespace DriveQuest.Data;

public static class DataStore
{
    public static List<Car> Cars { get; } = new();
    public static List<Customer> Customers { get; } = new();
    public static List<Rental> Rentals { get; } = new();
    public static int XP { get; private set; }
    private static readonly HashSet<string> ClaimedMissions = new();
    private static int nextCarId = 1, nextCustomerId = 1, nextRentalId = 1;

    public static void Seed()
    {
        if (Cars.Count > 0) return;
        Cars.AddRange(new[]
        {
            new Car(nextCarId++, "Toyota", "Vios", 2023, "Sedan", 1500),
            new Car(nextCarId++, "Honda", "Civic", 2022, "Sedan", 2200),
            new Car(nextCarId++, "Toyota", "Fortuner", 2023, "SUV", 3800),
            new Car(nextCarId++, "Mitsubishi", "Xpander", 2024, "MPV", 2700)
        });
        Customers.AddRange(new[]
        {
            new Customer(nextCustomerId++, "Alex Santos", "09171234567", "N01-23-456789"),
            new Customer(nextCustomerId++, "Jamie Cruz", "09181234567", "N02-23-654321")
        });
    }

    public static int Level => XP switch
    {
        < 100 => 1,
        < 250 => 2,
        < 500 => 3,
        < 1000 => 4,
        _ => 5
    };

    public static int LevelStart => Level switch { 1 => 0, 2 => 100, 3 => 250, 4 => 500, _ => 1000 };
    public static int LevelEnd => Level switch { 1 => 100, 2 => 250, 3 => 500, 4 => 1000, _ => 1000 };
    public static bool MissionClaimed(string key) => ClaimedMissions.Contains(key);

    public static decimal CompletedRevenue
    {
        get
        {
            decimal total = 0;
            foreach (Rental rental in Rentals)
            {
                if (rental.Status == "Completed") total += rental.TotalAmount;
            }
            return total;
        }
    }

    private static void CheckMissions()
    {
        if (Rentals.Count >= 1 && ClaimedMissions.Add("rental")) XP += 50;
        if (Cars.Count >= 5 && ClaimedMissions.Add("fleet")) XP += 50;
        if (Customers.Count >= 3 && ClaimedMissions.Add("customer")) XP += 50;
    }

    public static Car AddCar(string brand, string model, int year, string category, decimal price)
    {
        if (string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Brand and model are required.");
        if (year < 1980 || year > DateTime.Now.Year + 1)
            throw new ArgumentException("Enter a valid car year.");
        if (price <= 0) throw new ArgumentException("Price per day must be greater than zero.");
        Car car = new(nextCarId++, brand.Trim(), model.Trim(), year, category, price);
        Cars.Add(car);
        XP += 10;
        CheckMissions();
        return car;
    }

    public static void UpdateCar(int id, string brand, string model, int year, string category, decimal price)
    {
        Car car = Cars.FirstOrDefault(c => c.CarID == id) ?? throw new ArgumentException("Select a car first.");
        if (string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Brand and model are required.");
        if (year < 1980 || year > DateTime.Now.Year + 1)
            throw new ArgumentException("Enter a valid car year.");
        if (price <= 0) throw new ArgumentException("Price per day must be greater than zero.");
        car.Brand = brand.Trim(); car.Model = model.Trim(); car.Year = year;
        car.Category = category; car.PricePerDay = price;
    }

    public static void DeleteCar(int id)
    {
        Car car = Cars.FirstOrDefault(c => c.CarID == id) ?? throw new ArgumentException("Select a car first.");
        if (car.Status == "Rented") throw new InvalidOperationException("A rented car cannot be deleted.");
        if (Rentals.Any(r => r.CarID == id)) throw new InvalidOperationException("This car has rental history and cannot be deleted.");
        Cars.Remove(car);
    }

    public static Customer AddCustomer(string name, string phone, string license)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(license))
            throw new ArgumentException("Complete all customer fields.");
        Customer customer = new(nextCustomerId++, name.Trim(), phone.Trim(), license.Trim());
        Customers.Add(customer);
        XP += 10;
        CheckMissions();
        return customer;
    }

    public static void UpdateCustomer(int id, string name, string phone, string license)
    {
        Customer customer = Customers.FirstOrDefault(c => c.CustomerID == id) ?? throw new ArgumentException("Select a customer first.");
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(license))
            throw new ArgumentException("Complete all customer fields.");
        customer.FullName = name.Trim(); customer.Phone = phone.Trim(); customer.LicenseNumber = license.Trim();
    }

    public static void DeleteCustomer(int id)
    {
        Customer customer = Customers.FirstOrDefault(c => c.CustomerID == id) ?? throw new ArgumentException("Select a customer first.");
        if (Rentals.Any(r => r.CustomerID == id)) throw new InvalidOperationException("This customer has rental history and cannot be deleted.");
        Customers.Remove(customer);
    }

    public static Rental CreateRental(int customerId, int carId, int days)
    {
        if (!Customers.Any(c => c.CustomerID == customerId)) throw new ArgumentException("Choose a customer.");
        Car car = Cars.FirstOrDefault(c => c.CarID == carId) ?? throw new ArgumentException("Choose a car.");
        bool isAvailable = car.Status == "Available";
        if (!isAvailable) throw new InvalidOperationException("This car is already rented.");
        if (days < 1 || days > 365) throw new ArgumentException("Rental days must be between 1 and 365.");
        Rental rental = new(nextRentalId++, customerId, carId, days, car.PricePerDay * days);
        Rentals.Add(rental);
        car.Status = "Rented";
        XP += 50;
        CheckMissions();
        return rental;
    }

    public static void ReturnCar(int rentalId)
    {
        Rental rental = Rentals.FirstOrDefault(r => r.RentalID == rentalId) ?? throw new ArgumentException("Select a rental first.");
        if (rental.Status != "Active") throw new InvalidOperationException("This rental is already completed.");
        Car car = Cars.First(c => c.CarID == rental.CarID);
        rental.Status = "Completed";
        rental.ReturnDate = DateTime.Now;
        car.Status = "Available";
        XP += 25;
    }
}
