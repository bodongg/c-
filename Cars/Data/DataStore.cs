using AUTODOK.Models;

namespace AUTODOK.Data;

// Temporary storage for the current run. Restarting the app restores sample data.
public static class DataStore
{
    private static readonly object Sync = new();
    public static List<Car> Cars { get; } = new();
    public static List<Customer> Customers { get; } = new();
    public static List<Rental> Rentals { get; } = new();
    public static List<Payment> Payments { get; } = new();
    public static int XP { get; private set; }
    private static int nextCarId = 1, nextCustomerId = 1, nextRentalId = 1, nextPaymentId = 1;
    private static readonly HashSet<string> ClaimedMissions = new();

    public static void Seed()
    {
        lock (Sync)
        {
            if (Cars.Count > 0) return;
            AddSampleCar("Toyota", "Vios", "NBG 1234", 2023, "Sedan", 5, 1500, 1,
                "An efficient, easy-to-drive sedan for city trips and everyday travel.");
            AddSampleCar("Toyota", "Wigo", "NDF 5678", 2023, "Hatchback", 5, 1300, 2,
                "Compact and agile, ideal for navigating tight city streets.");
            AddSampleCar("Toyota", "Innova", "NCA 9012", 2022, "MPV", 7, 2500, 3,
                "A roomy three-row ride with comfort for families and groups.");
            AddSampleCar("Toyota", "Fortuner", "NGA 3456", 2023, "SUV", 7, 3500, 4,
                "A capable SUV for long-distance travel and family adventures.");
            AddSampleCar("Mitsubishi", "Xpander", "NHD 7890", 2024, "MPV", 7, 2500, 5,
                "A versatile seven-seater with space for extended trips.");
            AddSampleCar("Honda", "City", "NIX 2468", 2023, "Sedan", 5, 1800, 6,
                "A sleek sedan that balances comfort, style, and fuel economy.");
            AddSampleCar("Ford", "Raptor", "NOM 1357", 2023, "Pickup", 5, 3000, 7,
                "A rugged pickup suited to open roads and outdoor weekends.");
            AddSampleCar("Toyota", "Avanza", "NLP 8642", 2022, "MPV", 7, 2000, 8,
                "An affordable multi-purpose vehicle for families on the move.");
            Customers.Add(new Customer { CustomerID = nextCustomerId++, FullName = "Alex Santos", Phone = "09171234567", LicenseNumber = "N01-23-456789", IsSample = true });
            Customers.Add(new Customer { CustomerID = nextCustomerId++, FullName = "Jamie Cruz", Phone = "09181234567", LicenseNumber = "N02-23-654321", IsSample = true });
            Customers.Add(new Customer { CustomerID = nextCustomerId++, FullName = "Pat Reyes", Phone = "09191234567", LicenseNumber = "N03-23-123456", IsSample = true });
            AddSampleRental(1, 1, 2, -1, "Active", "Cash", "Pending");
            AddSampleRental(2, 6, 2, -5, "Completed", "GCash", "Paid");
            AddSampleRental(3, 4, 3, -9, "Completed", "Card", "Paid");
        }
    }

    private static void AddSampleCar(string brand, string model, string plate, int year, string category, int seats, decimal price, int slot, string description)
        => Cars.Add(new Car { CarID = nextCarId++, Brand = brand, Model = model, LicensePlate = plate, Year = year, Category = category,
            Seats = seats, PricePerDay = price, ImageSlot = slot, Description = description, IsSample = true });

    private static void AddSampleRental(int customerId, int carId, int days, int startOffset, string status, string method, string paymentStatus)
    {
        Car car = Cars.First(c => c.CarID == carId);
        DateTime pickup = DateTime.Today.AddDays(startOffset);
        Rental rental = new() { RentalID = nextRentalId++, CustomerID = customerId, CarID = carId,
            RentalDate = pickup.AddDays(-1), PickupDate = pickup, PlannedReturnDate = pickup.AddDays(days),
            NumberOfDays = days, BaseAmount = car.PricePerDay * days, TotalAmount = car.PricePerDay * days,
            PaymentMethod = method, Status = status, IsSample = true,
            ReturnDate = status == "Completed" ? pickup.AddDays(days) : null };
        Rentals.Add(rental);
        if (status == "Active") car.Status = "Rented";
        Payments.Add(new Payment { PaymentID = nextPaymentId++, RentalID = rental.RentalID, Amount = rental.TotalAmount,
            Method = method, Status = paymentStatus, PaidDate = paymentStatus == "Paid" ? pickup : null });
    }

    public static List<Car> CarList()
    {
        lock (Sync) return Cars.Select(c => Copy(c)).ToList();
    }
    public static List<Customer> CustomerList()
    {
        lock (Sync) return Customers.Select(c => Copy(c)).ToList();
    }
    public static List<Rental> RentalList()
    {
        lock (Sync) return Rentals.Select(r => Copy(r)).ToList();
    }
    public static List<Payment> PaymentList()
    {
        lock (Sync) return Payments.Select(p => Copy(p)).ToList();
    }
    public static Car? FindCar(int id)
    {
        lock (Sync) { Car? car = Cars.FirstOrDefault(c => c.CarID == id); return car == null ? null : Copy(car); }
    }
    public static Rental? FindRental(int id)
    {
        lock (Sync) { Rental? rental = Rentals.FirstOrDefault(r => r.RentalID == id); return rental == null ? null : Copy(rental); }
    }
    public static Customer? FindCustomer(int id)
    {
        lock (Sync) { Customer? customer = Customers.FirstOrDefault(c => c.CustomerID == id); return customer == null ? null : Copy(customer); }
    }

    public static Car AddCar(string brand, string model, string licensePlate, int year, string category, string transmission,
        int seats, decimal price, string description, int imageSlot)
    {
        ValidateCar(brand, model, licensePlate, year, category, transmission, seats, price, imageSlot);
        lock (Sync)
        {
            if (Cars.Any(c => c.LicensePlate.Equals(licensePlate.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("This license plate is already in the fleet.");
            Car car = new() { CarID = nextCarId++, Brand = brand.Trim(), Model = model.Trim(), Year = year,
                LicensePlate = licensePlate.Trim().ToUpperInvariant(), Category = category.Trim(), Transmission = transmission.Trim(), Seats = seats, PricePerDay = price,
                Description = description.Trim(), ImageSlot = imageSlot };
            Cars.Add(car);
            XP += 10;
            CheckMissions();
            return Copy(car);
        }
    }

    public static void UpdateCar(int id, string brand, string model, string licensePlate, int year, string category, string transmission,
        int seats, decimal price, string description, int imageSlot)
    {
        ValidateCar(brand, model, licensePlate, year, category, transmission, seats, price, imageSlot);
        lock (Sync)
        {
            Car car = Cars.FirstOrDefault(c => c.CarID == id) ?? throw new ArgumentException("Choose a car to edit.");
            if (Cars.Any(c => c.CarID != id && c.LicensePlate.Equals(licensePlate.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Another car already has this license plate.");
            car.Brand = brand.Trim(); car.Model = model.Trim(); car.Year = year; car.Category = category.Trim();
            car.LicensePlate = licensePlate.Trim().ToUpperInvariant(); car.Transmission = transmission.Trim(); car.Seats = seats; car.PricePerDay = price;
            car.Description = description.Trim(); car.ImageSlot = imageSlot;
        }
    }

    public static void DeleteCar(int id)
    {
        lock (Sync)
        {
            Car car = Cars.FirstOrDefault(c => c.CarID == id) ?? throw new ArgumentException("Choose a car to delete.");
            if (car.Status == "Rented") throw new InvalidOperationException("Return this car before deleting it.");
            if (Rentals.Any(r => r.CarID == id)) throw new InvalidOperationException("This car has rental history and cannot be deleted.");
            Cars.Remove(car);
        }
    }

    public static void SetMaintenance(int id, bool maintenance)
    {
        lock (Sync)
        {
            Car car = Cars.FirstOrDefault(c => c.CarID == id) ?? throw new ArgumentException("Car not found.");
            if (car.Status == "Rented") throw new InvalidOperationException("Return this car before changing maintenance status.");
            car.Status = maintenance ? "Maintenance" : "Available";
        }
    }

    public static Customer AddCustomer(string name, string phone, string license)
    {
        ValidateCustomer(name, phone, license);
        lock (Sync)
        {
            if (Customers.Any(c => c.Phone.Equals(phone.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("A customer with this phone number already exists.");
            Customer customer = new() { CustomerID = nextCustomerId++, FullName = name.Trim(), Phone = phone.Trim(), LicenseNumber = license.Trim() };
            Customers.Add(customer); XP += 10; CheckMissions();
            return Copy(customer);
        }
    }

    public static void UpdateCustomer(int id, string name, string phone, string license)
    {
        ValidateCustomer(name, phone, license);
        lock (Sync)
        {
            Customer customer = Customers.FirstOrDefault(c => c.CustomerID == id) ?? throw new ArgumentException("Choose a customer to edit.");
            if (Customers.Any(c => c.CustomerID != id && c.Phone.Equals(phone.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Another customer already has this phone number.");
            customer.FullName = name.Trim(); customer.Phone = phone.Trim(); customer.LicenseNumber = license.Trim();
        }
    }

    public static void DeleteCustomer(int id)
    {
        lock (Sync)
        {
            Customer customer = Customers.FirstOrDefault(c => c.CustomerID == id) ?? throw new ArgumentException("Choose a customer to delete.");
            if (Rentals.Any(r => r.CustomerID == id)) throw new InvalidOperationException("This customer has rental history and cannot be deleted.");
            Customers.Remove(customer);
        }
    }

    public static Rental BookCar(int carId, string name, string phone, string license,
        DateTime pickup, DateTime plannedReturn, string pickupLocation, string returnLocation,
        bool insurance, string paymentMethod)
    {
        ValidateCustomer(name, phone, license);
        if (pickup.Date < DateTime.Today) throw new ArgumentException("Choose today or a future pickup date.");
        int days = (plannedReturn.Date - pickup.Date).Days;
        if (days < 1 || days > 30) throw new ArgumentException("Rental length must be between 1 and 30 days.");
        if (string.IsNullOrWhiteSpace(pickupLocation) || string.IsNullOrWhiteSpace(returnLocation))
            throw new ArgumentException("Choose pickup and return locations.");
        if (paymentMethod is not ("Cash" or "GCash" or "Card")) throw new ArgumentException("Choose a payment method.");
        lock (Sync)
        {
            Car car = Cars.FirstOrDefault(c => c.CarID == carId) ?? throw new ArgumentException("Car not found.");
            if (car.Status != "Available") throw new InvalidOperationException("This car is already rented. Choose another car.");
            Customer? customer = Customers.FirstOrDefault(c => c.Phone.Equals(phone.Trim(), StringComparison.OrdinalIgnoreCase));
            if (customer == null)
            {
                customer = new Customer { CustomerID = nextCustomerId++, FullName = name.Trim(), Phone = phone.Trim(), LicenseNumber = license.Trim() };
                Customers.Add(customer); XP += 10;
            }
            else if (!customer.LicenseNumber.Equals(license.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The license number does not match the existing customer with this phone number.");
            decimal baseAmount = car.PricePerDay * days;
            decimal insuranceFee = insurance ? 500m : 0m;
            Rental rental = new() { RentalID = nextRentalId++, CarID = carId, CustomerID = customer.CustomerID,
                RentalDate = DateTime.Now, PickupDate = pickup.Date, PlannedReturnDate = plannedReturn.Date,
                PickupLocation = pickupLocation.Trim(), ReturnLocation = returnLocation.Trim(),
                NumberOfDays = days, BaseAmount = baseAmount, InsuranceFee = insuranceFee,
                TotalAmount = baseAmount + insuranceFee, PaymentMethod = paymentMethod, Status = "Active" };
            Rentals.Add(rental); car.Status = "Rented";
            Payments.Add(new Payment { PaymentID = nextPaymentId++, RentalID = rental.RentalID,
                Amount = rental.TotalAmount, Method = paymentMethod, Status = "Pending" });
            XP += 50; CheckMissions();
            return Copy(rental);
        }
    }

    public static void ReturnCar(int rentalId)
    {
        lock (Sync)
        {
            Rental rental = Rentals.FirstOrDefault(r => r.RentalID == rentalId) ?? throw new ArgumentException("Rental not found.");
            if (rental.Status != "Active") throw new InvalidOperationException("This rental has already been returned.");
            Car car = Cars.First(c => c.CarID == rental.CarID);
            rental.Status = "Completed"; rental.ReturnDate = DateTime.Now; car.Status = "Available"; XP += 25;
        }
    }

    public static void CancelRental(int rentalId)
    {
        lock (Sync)
        {
            Rental rental = Rentals.FirstOrDefault(r => r.RentalID == rentalId) ?? throw new ArgumentException("Rental not found.");
            if (rental.Status != "Active") throw new InvalidOperationException("Only active rentals can be cancelled.");
            Car car = Cars.First(c => c.CarID == rental.CarID);
            rental.Status = "Cancelled"; car.Status = "Available";
            Payment? payment = Payments.FirstOrDefault(p => p.RentalID == rentalId);
            if (payment is { Status: "Pending" }) payment.Status = "Cancelled";
        }
    }

    public static void RecordPayment(int paymentId)
    {
        lock (Sync)
        {
            Payment payment = Payments.FirstOrDefault(p => p.PaymentID == paymentId) ?? throw new ArgumentException("Payment not found.");
            if (payment.Status != "Pending") throw new InvalidOperationException("Only pending payments can be recorded as paid.");
            if (Rentals.Any(r => r.RentalID == payment.RentalID && r.Status == "Cancelled"))
                throw new InvalidOperationException("A cancelled booking cannot be marked paid.");
            payment.Status = "Paid"; payment.PaidDate = DateTime.Now;
        }
    }

    public static void RefundPayment(int paymentId)
    {
        lock (Sync)
        {
            Payment payment = Payments.FirstOrDefault(p => p.PaymentID == paymentId) ?? throw new ArgumentException("Payment not found.");
            if (payment.Status != "Paid") throw new InvalidOperationException("Only paid payments can be marked refunded.");
            payment.Status = "Refunded";
        }
    }

    public static int Level => XP switch { < 100 => 1, < 250 => 2, < 500 => 3, < 1000 => 4, _ => 5 };
    public static int LevelStart => Level switch { 1 => 0, 2 => 100, 3 => 250, 4 => 500, _ => 1000 };
    public static int LevelEnd => Level switch { 1 => 100, 2 => 250, 3 => 500, 4 => 1000, _ => 1000 };
    public static bool MissionClaimed(string key) { lock (Sync) return ClaimedMissions.Contains(key); }
    public static int AddedCarCount { get { lock (Sync) return Cars.Count(c => !c.IsSample); } }
    public static int AddedCustomerCount { get { lock (Sync) return Customers.Count(c => !c.IsSample); } }
    public static int AvailableCount { get { lock (Sync) return Cars.Count(c => c.Status == "Available"); } }
    public static int ActiveRentalCount { get { lock (Sync) return Rentals.Count(r => r.Status == "Active"); } }
    public static int TotalRentalCount { get { lock (Sync) return Rentals.Count; } }
    public static int CustomerCount { get { lock (Sync) return Customers.Count; } }
    public static decimal CompletedRevenue
    {
        get { lock (Sync) { decimal total = 0; foreach (Rental r in Rentals) if (r.Status == "Completed") total += r.TotalAmount; return total; } }
    }

    private static void CheckMissions()
    {
        if (Rentals.Count(r => !r.IsSample) >= 1 && ClaimedMissions.Add("rental")) XP += 50;
        if (Cars.Count(c => !c.IsSample) >= 5 && ClaimedMissions.Add("fleet")) XP += 50;
        if (Customers.Count(c => !c.IsSample) >= 3 && ClaimedMissions.Add("customer")) XP += 50;
    }

    private static void ValidateCar(string brand, string model, string licensePlate, int year, string category, string transmission, int seats, decimal price, int imageSlot)
    {
        if (string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(licensePlate))
            throw new ArgumentException("Brand, model, license plate, and category are required.");
        if (year < 1980 || year > DateTime.Today.Year + 1) throw new ArgumentException("Enter a valid year.");
        if (transmission is not ("Automatic" or "Manual")) throw new ArgumentException("Choose Automatic or Manual transmission.");
        if (seats < 2 || seats > 15) throw new ArgumentException("Seats must be between 2 and 15.");
        if (price <= 0) throw new ArgumentException("Price per day must be greater than zero.");
        if (imageSlot < 1 || imageSlot > 8) throw new ArgumentException("Choose a photo from 1 to 8.");
    }
    private static void ValidateCustomer(string name, string phone, string license)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(license))
            throw new ArgumentException("Name, phone, and license number are required.");
        if (phone.Trim().Length < 7) throw new ArgumentException("Enter a valid phone number.");
    }

    private static Car Copy(Car c) => new() { CarID = c.CarID, Brand = c.Brand, Model = c.Model, LicensePlate = c.LicensePlate, Year = c.Year,
        Category = c.Category, Transmission = c.Transmission, Seats = c.Seats, PricePerDay = c.PricePerDay,
        Description = c.Description, Status = c.Status, ImageSlot = c.ImageSlot, IsSample = c.IsSample };
    private static Customer Copy(Customer c) => new() { CustomerID = c.CustomerID, FullName = c.FullName,
        Phone = c.Phone, LicenseNumber = c.LicenseNumber, IsSample = c.IsSample };
    private static Rental Copy(Rental r) => new() { RentalID = r.RentalID, CustomerID = r.CustomerID, CarID = r.CarID,
        RentalDate = r.RentalDate, PickupDate = r.PickupDate, PlannedReturnDate = r.PlannedReturnDate,
        PickupLocation = r.PickupLocation, ReturnLocation = r.ReturnLocation,
        ReturnDate = r.ReturnDate, NumberOfDays = r.NumberOfDays, BaseAmount = r.BaseAmount,
        InsuranceFee = r.InsuranceFee, TotalAmount = r.TotalAmount, PaymentMethod = r.PaymentMethod, Status = r.Status, IsSample = r.IsSample };
    private static Payment Copy(Payment p) => new() { PaymentID = p.PaymentID, RentalID = p.RentalID, Amount = p.Amount,
        Method = p.Method, Status = p.Status, PaidDate = p.PaidDate };
}
