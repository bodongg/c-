# AUTODOK — Car Rental Management System

This is a C# ASP.NET Core Razor Pages project for a school presentation. The customer catalog follows the supplied eight-car reference image. Admin login, dashboard, vehicles, booking, reservations, and payments follow the six supplied UI screenshots with their colors changed to charcoal and orange. **There is no SQL, database, Entity Framework, API, or internet dependency.** C# `List<T>` collections hold data only while the app is running.

## Open in your IDE

1. Install the **.NET 10 SDK** on a Windows computer. Use a Visual Studio version that supports .NET 10, or VS Code with C# Dev Kit.
2. Open the `Cars` folder or `Cars/AUTODOK.csproj`.
3. In a terminal inside the `Cars` folder, run `dotnet run`.
4. Open the localhost address printed by the terminal. The included run profile uses `http://localhost:5214` for the classroom demo.
5. Browse the eight cars on the customer home page. Click **Admin** and log in with `admin` / `admin123`.

No NuGet packages need to be added to this project. The eight-car photo sheet, login photo, and supplied AUTODOK logo are bundled in `wwwroot/images` so the website works without internet access. Admin can upload a PNG, JPEG, or WebP vehicle photo up to 2 MB; it stays in memory and resets with the car data when the app restarts.

## File structure

```text
Cars/
├── AUTODOK.csproj
├── Program.cs
├── Models/
│   ├── Car.cs
│   ├── Customer.cs
│   ├── Rental.cs
│   └── Payment.cs
├── Data/
│   └── DataStore.cs
├── Services/
│   └── VehiclePhoto.cs              validates vehicle uploads
├── Pages/
│   ├── Index.cshtml(.cs)             catalog
│   ├── Cars/Details.cshtml(.cs)      car details
│   ├── Bookings/Create.cshtml(.cs)   booking + live total
│   ├── Bookings/Confirmation.cshtml(.cs)
│   ├── Bookings/MyRentals.cshtml(.cs)
│   ├── Admin/Login.cshtml(.cs)
│   ├── Admin/Logout.cshtml(.cs)
│   ├── Admin/Index.cshtml(.cs)       dashboard
│   ├── Admin/Bookings.cshtml(.cs)    five-step booking
│   ├── Admin/Cars.cshtml(.cs)        vehicle CRUD
│   ├── Admin/Customers.cshtml(.cs)   customer CRUD
│   ├── Admin/Reservations.cshtml(.cs) return + history
│   ├── Admin/Payments.cshtml(.cs)    payment records
│   ├── Admin/Receipt.cshtml(.cs)     print / save receipt
│   └── Shared/                     site and admin layouts
└── wwwroot/
    ├── css/site.css, admin.css
    ├── js/site.js
    └── images/car-sheet.png, login-fleet.png, autodok-logo.png
```

## How the data flows

`Program.cs` calls `DataStore.Seed()` at startup. The customer pages and admin pages call `DataStore` methods to read or change the same in-memory lists. Booking checks availability, calculates `(return date − pickup date) × price per day + optional ₱500 insurance`, creates a `Rental` and a pending `Payment`, and changes the car to **Rented**. Admin can mark a payment paid or refunded as a demo record; no money moves. Returning changes the rental to **Completed**, records the actual return time, and changes the car to **Available**. Dashboard revenue sums completed rentals only.

