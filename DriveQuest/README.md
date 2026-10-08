# DRIVE QUEST — Gamified Car Rental Management System

A beginner-friendly Windows Forms final project. All data stays in `List<T>` collections while the program runs. There is no SQL, database, API, or internet requirement.

## Project structure

```text
DriveQuest/
├── DriveQuest.csproj
├── Program.cs
├── Data/
│   └── DataStore.cs
├── Models/
│   ├── Car.cs
│   ├── Customer.cs
│   └── Rental.cs
└── Forms/
    ├── Ui.cs
    ├── LoginForm.cs
    ├── DashboardForm.cs
    ├── CarsForm.cs
    ├── CustomersForm.cs
    ├── RentalsForm.cs
    └── ReturnCarForm.cs
```

## Open and run in Visual Studio

1. On a **Windows** computer, install Visual Studio 2022 with the **.NET desktop development** workload and .NET 8 SDK.
2. Copy the entire `DriveQuest` folder to the computer.
3. In Visual Studio, choose **File → Open → Project/Solution** and select `DriveQuest.csproj`.
4. Press **F5**. If Visual Studio asks to trust or restore the project, allow it. No third-party NuGet packages are used.
5. Log in with username `admin` and password `admin123`.

All forms build their controls directly in C# constructors. There are no `.Designer.cs` files to add or reconnect. `Program.cs` starts `LoginForm`, and the login opens `DashboardForm`. Dashboard buttons open the relevant forms. `ReturnCarForm` also provides rental history through its filter.

To create this from scratch instead, create a **Windows Forms App** in Visual Studio, choose **.NET 8**, name it `DriveQuest`, then copy the files from this folder into the new project. Remove the default `Form1.cs` and replace the generated `Program.cs` with the one here. Keep the namespaces as written.

## Quick demo sequence

1. Log in. Explain that sample cars and customers are inserted by `DataStore.Seed()`.
2. On the dashboard, point out the statistics, XP bar, and missions.
3. Open **Cars**. Show the sample cars, search for Toyota, then clear.
4. Add a Ford Everest (year 2024, SUV, ₱3,900/day). This reaches the five-car mission. Select it, edit its price, and show that the grid changes.
5. Open **Customers**. Add a third customer. This reaches the customer mission.
6. Open **Rent a Car**. Select the new customer and the Ford Everest. Change days to 3; show the live total calculation. Confirm.
7. Open **Cars** again; the Ford Everest is now **Rented**.
8. Open **Return Car**. Select the active rental and return it.
9. Open **Rental History**. Use the All, Active, and Completed filters.
10. Return to the dashboard. Show revenue from completed rentals and the updated XP and level.

Data resets when the application closes. That is expected for an in-memory project.

## Five-hour implementation checklist

| Time | Work |
|---|---|
| 0:00–0:30 | Install/open Visual Studio, create/open project, run login screen |
| 0:30–1:15 | Understand `Car`, `Customer`, `Rental`, and `DataStore` |
| 1:15–2:15 | Test car and customer Add, Update, Delete, Search |
| 2:15–3:10 | Test price calculation, rental creation, and car status |
| 3:10–3:50 | Test return, rental history, and completed revenue |
| 3:50–4:20 | Test dashboard, XP, levels, and missions |
| 4:20–5:00 | Rehearse the demo and fix any issues on the presentation computer |

## Five-minute presentation script

**0:00–0:40 — Introduction.** “Drive Quest is a gamified car rental management system built with C# Windows Forms. It uses classes and in-memory lists rather than a database, so the data exists for the current run.” Log in as admin.

**0:40–1:20 — Dashboard.** “This dashboard calculates available cars, active rentals, customers, and revenue from our lists. Users also earn XP and complete missions.” Show the starting values.

**1:20–2:10 — CRUD.** Open Cars. “This grid reads from our car list. We can add, edit, delete, and search.” Add a car, edit it, and search. Open Customers and add a customer. Mention validation and duplicate-free generated IDs.

**2:10–3:15 — Rental.** Open Rent a Car. “Only available cars appear here. The total updates automatically using price per day times number of days.” Change the days and confirm. Show that the car becomes Rented and XP increases.

**3:15–4:10 — Return and history.** Return the active rental. “The rental becomes Completed, gets a return date, and the car becomes Available again.” Open Rental History and filter to Completed.

**4:10–5:00 — Concepts and closing.** Return to Dashboard. “The completed rental now counts toward revenue. We demonstrated classes, objects, lists, LINQ, event handlers, validation, CRUD, transactions, and exception handling. The game elements reward normal rental work.”

## C# concepts by feature

| Feature | Concepts shown |
|---|---|
| Models | Classes, constructors, properties, objects, `int`, `string`, `decimal`, `DateTime` |
| DataStore | Static state, `List<T>`, encapsulated methods, IDs, conditions, `switch`, LINQ |
| Cars and Customers | CRUD, search, grid binding, LINQ, event handlers, validation, exceptions |
| Rental | Object creation, multiplication, `bool` conditions, state changes, events, exception handling |
| Return and history | Nullable `DateTime`, filtering, state transitions, lookups |
| Dashboard | `Count`, `foreach` revenue calculation, derived values, XP levels, mission checks, progress bar |

## Limits to explain honestly

This is a classroom demonstration. Passwords are hardcoded, records are not saved after exit, and there is no multi-user support. The total uses the booked number of days; it does not recalculate from the actual return time.
