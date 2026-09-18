using System;
using System.Globalization;
using System.Collections.Generic;
using Agullto_IMS.Models;
using Agullto_IMS.Data;
using Agullto_IMS.Services;
using Microsoft.Extensions.Configuration;

namespace Agullto_IMS
{
    class Program
    {
        // Service and Configuration Initializations
        static IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        static ProductService productService = new ProductService(new InventoryData());
        static EmailService emailService = new EmailService(configuration);
        static string adminEmail = "admin@example.com"; // Replace with your target email address

        static List<UserAccount> mockAccounts = new List<UserAccount>
        {
            new UserAccount { Username = "admin", Password = "123", Role = "Admin" },
            new UserAccount { Username = "emp", Password = "123", Role = "Employee" }
        };

        static List<AccessLog> accessLogs = new List<AccessLog>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== AGULLTO IMS & GROCERY SYSTEM INTEGRATION =====");
                Console.Write("Do you want to login? [Y]|[N]: ");
                string entry = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (entry == "N")
                {
                    Console.WriteLine("System Shutdown.");
                    break;
                }
                if (entry == "Y")
                {
                    ExecuteLoginSession();
                }
            }
        }

        static void ExecuteLoginSession()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Enter password: ");
            string password = Console.ReadLine()?.Trim() ?? "";

            var account = mockAccounts.Find(a => a.Username == username && a.Password == password);

            if (account == null)
            {
                Console.WriteLine("Invalid Credentials! Returning to main menu.");
                accessLogs.Add(new AccessLog { Username = username, Role = "None", Status = false, Timestamp = DateTime.Now });
                Console.ReadLine();
                return;
            }

            accessLogs.Add(new AccessLog { Username = username, Role = account.Role, Status = true, Timestamp = DateTime.Now });
            Console.WriteLine($"\nWelcome {account.Username} ({account.Role})!");
            Console.WriteLine("Press Enter to access menus...");
            Console.ReadLine();

            if (account.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                ShowAdminMenu();
            }
            else
            {
                ShowEmployeeMenu();
            }
        }

        static void ShowAdminMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("========= ADMIN PRIVILEGED CONSOLE =========");
                Console.WriteLine("[1] Create New Advanced Grocery Product");
                Console.WriteLine("[2] View All Synchronized Inventory Records");
                Console.WriteLine("[3] Update Target Record Details");
                Console.WriteLine("[4] Delete Entry Row");
                Console.WriteLine("[5] View Authentication Access Logs");
                Console.WriteLine("[6] Logout");
                Console.Write("Choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                if (choice == 1) AddGroceryProductFlow();
                else if (choice == 2) DisplayAllInventoryFlow();
                else if (choice == 3) UpdateProductFlow();
                else if (choice == 4) DeleteProductFlow();
                else if (choice == 5) DisplayLogsFlow();
                else if (choice == 6) break;

                CheckLowStockAlerts();
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }

        static void ShowEmployeeMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("========= EMPLOYEE CONSOLE =========");
                Console.WriteLine("[1] View All Synchronized Inventory Records");
                Console.WriteLine("[2] Run Specific Product Search Lookup");
                Console.WriteLine("[3] Logout");
                Console.Write("Choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                if (choice == 1) DisplayAllInventoryFlow();
                else if (choice == 2) SearchProductFlow();
                else if (choice == 3) break;

                CheckLowStockAlerts();
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }

        // --- Core CRUD Flow Logic ---

        static void AddGroceryProductFlow()
        {
            Console.Clear();
            Console.WriteLine(">>> Add New Record <<<");

            Console.Write("Item Name: ");
            string name = Console.ReadLine()?.Trim() ?? "Unnamed Product";

            int stock = ReadIntInput("Stock Level: ");
            decimal cost = ReadDecimalInput("Cost Price (Wholesale value): ");
            decimal selling = ReadDecimalInput("Selling Retail Price (Consumer value): ");

            // Weight Input Prompt
            double weight = ReadDoubleInput("Weight/Quantity Value (e.g. 1.5, 500): ");

            // Measurement Unit Selection
            Console.WriteLine("\nSelect Measurement Unit:");
            Console.WriteLine("[0] Pcs | [1] Kg | [2] Grams | [3] Liter | [4] Ml");
            int unitChoice = ReadIntInput("Unit Choice: ");
            MeasurementUnit unit = Enum.IsDefined(typeof(MeasurementUnit), unitChoice)
                ? (MeasurementUnit)unitChoice
                : MeasurementUnit.Pcs;

            // Department Selection
            Console.WriteLine("\nSelect Department:");
            Console.WriteLine("[0] Pantry | [1] Dairy | [2] Produce | [3] Bakery | [4] Beverages");
            int deptChoice = ReadIntInput("Department Choice: ");
            ProductDepartment dept = Enum.IsDefined(typeof(ProductDepartment), deptChoice)
                ? (ProductDepartment)deptChoice
                : ProductDepartment.Pantry;

            Console.Write("\nLocation Layout/Aisle coordinate: ");
            string shelf = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(shelf)) shelf = "Aisle 1";

            Product newProd = new Product
            {
                Name = name,
                Stock = stock,
                CostPrice = cost,
                SellingPrice = selling,
                WeightValue = weight,
                Unit = unit,
                Department = dept,
                Location = shelf
            };

            // 1. Save product
            productService.AddProduct(newProd);
            Console.WriteLine("\nRecord committed successfully.");

            // 2. Dispatch minimal email confirmation
            try
            {
                Console.WriteLine("Sending email notification...");
                emailService.SendEmail(newProd, adminEmail);
                Console.WriteLine("Email notification sent successfully!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Warning] Product saved, but email failed: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void DisplayAllInventoryFlow()
        {
            Console.Clear();
            var systemList = productService.GetAllProducts();

            Console.WriteLine("=== Current Balanced Multi-Storage Ledger ===");
            if (systemList == null || systemList.Count == 0)
            {
                Console.WriteLine("No available metrics saved.");
                return;
            }

            foreach (var item in systemList)
            {
                Console.WriteLine($"ID: {item.Id}\n - Name: {item.Name} | Stock: {item.Stock} {item.Unit} | Location: {item.Location}\n - Weight: {item.WeightValue} | Cost: {item.CostPrice:C} | Retail Sale: {item.SellingPrice:C}\n");
            }
        }

        static void SearchProductFlow()
        {
            Console.Write("Enter Product ID to look up: ");
            if (Guid.TryParse(Console.ReadLine(), out Guid searchId))
            {
                var target = productService.FindProduct(searchId);
                if (target != null)
                {
                    Console.WriteLine($"\nMatch Found: {target.Name} -> Stocks left: {target.Stock} units inside {target.Location}");
                }
                else Console.WriteLine("No records tracking that identifier configuration found.");
            }
            else
            {
                Console.WriteLine("Invalid GUID format.");
            }
        }

        static void UpdateProductFlow()
        {
            DisplayAllInventoryFlow();
            Console.Write("Enter Target Guid ID to update: ");
            if (Guid.TryParse(Console.ReadLine(), out Guid parsedId))
            {
                var target = productService.FindProduct(parsedId);
                if (target == null)
                {
                    Console.WriteLine("Product not found.");
                    return;
                }

                Console.Write($"New Name ({target.Name}): ");
                string newName = Console.ReadLine()?.Trim() ?? "";
                if (!string.IsNullOrEmpty(newName)) target.Name = newName;

                Console.Write($"Change Stocks Level ({target.Stock}): ");
                string stockInput = Console.ReadLine()?.Trim() ?? "";
                if (int.TryParse(stockInput, out int parsedStock))
                {
                    target.Stock = parsedStock;
                }

                productService.UpdateProduct(target);
                Console.WriteLine("Changes successfully applied across persistence mechanisms.");
            }
            else
            {
                Console.WriteLine("Invalid GUID format.");
            }
        }

        static void DeleteProductFlow()
        {
            DisplayAllInventoryFlow();
            Console.Write("Enter target system Guid to drop: ");
            if (Guid.TryParse(Console.ReadLine(), out Guid deletionId))
            {
                if (productService.DeleteProduct(deletionId))
                {
                    Console.WriteLine("Item safely deleted out of tracking matrices.");
                }
                else
                {
                    Console.WriteLine("Failed to delete product or record does not exist.");
                }
            }
            else
            {
                Console.WriteLine("Invalid GUID format.");
            }
        }

        static void DisplayLogsFlow()
        {
            Console.Clear();
            Console.WriteLine("=== SECURITY SESSIONS LOG ===");
            foreach (var log in accessLogs)
            {
                Console.WriteLine($"[{log.Timestamp:yyyy-MM-dd HH:mm:ss}] User: {log.Username} | Role Context: {log.Role} -> Authorized: {log.Status}");
            }
        }

        static void CheckLowStockAlerts()
        {
            var lowStock = productService.GetLowStockItems();
            if (lowStock != null && lowStock.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n⚠️  CRITICAL ALERT: LOW STOCK ITEMS DETECTED (< 5)");
                foreach (var item in lowStock)
                {
                    Console.WriteLine($" -> {item.Name} only has {item.Stock} units remaining in {item.Location}.");
                }
                Console.ResetColor();
            }
        }

        // --- Validation Input Helpers ---

        static int ReadIntInput(string prompt)
        {
            int result;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid integer value. Re-enter: ");
                Console.ResetColor();
            }
            return result;
        }

        static decimal ReadDecimalInput(string prompt)
        {
            decimal result;
            Console.Write(prompt);
            while (!decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid decimal value. Re-enter: ");
                Console.ResetColor();
            }
            return result;
        }

        static double ReadDoubleInput(string prompt)
        {
            double result;
            Console.Write(prompt);
            while (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid double/numeric value. Re-enter: ");
                Console.ResetColor();
            }
            return result;
        }
    }
}