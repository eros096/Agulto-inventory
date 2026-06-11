using System;
using System.Globalization;
using System.Collections.Generic;
using Agullto_IMS.Models;
using Agullto_IMS.Data;
using Agullto_IMS.Services;

namespace Agullto_IMS
{
    class Program
    {
        static ProductService productService = new ProductService(new InventoryData());

        // Mock Account Service for demonstration context
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
                string entry = Console.ReadLine()?.ToUpper() ?? "";

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
            string username = Console.ReadLine() ?? "";
            Console.Write("Enter password: ");
            string password = Console.ReadLine() ?? "";

            // Simple Authentication Check
            var account = mockAccounts.Find(a => a.Username == username && a.Password == password);

            if (account == null)
            {
                Console.WriteLine("Invalid Credentials! Returning to main menu.");
                accessLogs.Add(new AccessLog { Username = username, Role = "None", Status = false });
                Console.ReadLine();
                return;
            }

            accessLogs.Add(new AccessLog { Username = username, Role = account.Role, Status = true });
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

        // --- Core CRUD Presentation Layout Logic ---

        static void AddGroceryProductFlow()
        {
            Console.Clear();
            Console.WriteLine(">>> Add New Record <<<");
            Console.Write("Item Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Stock Level: ");
            int stock = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Cost Price (Wholesale value): ");
            decimal cost = decimal.Parse(Console.ReadLine() ?? "0");

            Console.Write("Selling Retail Price (Consumer value): ");
            decimal selling = decimal.Parse(Console.ReadLine() ?? "0");

            Console.Write("Location Layout/Aisle coordinate: ");
            string shelf = Console.ReadLine() ?? "Aisle 1";

            Product newProd = new Product
            {
                Name = name,
                Stock = stock,
                CostPrice = cost,
                SellingPrice = selling,
                Location = shelf,
                Department = ProductDepartment.Pantry, // Enums can be evaluated dynamically
                Unit = MeasurementUnit.Pcs
            };

            productService.AddProduct(newProd);
            Console.WriteLine("\nRecord committed and exported to SQL Server table + Products.json successfully.");
        }

        static void DisplayAllInventoryFlow()
        {
            Console.Clear();
            var systemList = productService.GetAllProducts();

            Console.WriteLine("=== Current Balanced Multi-Storage Ledger ===");
            if (systemList.Count == 0)
            {
                Console.WriteLine("No available metrics saved.");
                return;
            }

            foreach (var item in systemList)
            {
                Console.WriteLine($"ID: {item.Id}\n - Name: {item.Name} | Stock: {item.Stock} | Location: {item.Location}\n - Cost: {item.CostPrice:C} | Retail Sale: {item.SellingPrice:C}\n");
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
        }

        static void UpdateProductFlow()
        {
            DisplayAllInventoryFlow();
            Console.Write("Enter Target Guid ID to update: ");
            if (Guid.TryParse(Console.ReadLine(), out Guid parsedId))
            {
                var target = productService.FindProduct(parsedId);
                if (target == null) return;

                Console.Write($"New Name ({target.Name}): ");
                target.Name = Console.ReadLine() ?? target.Name;

                Console.Write($"Change Stocks Level ({target.Stock}): ");
                target.Stock = int.Parse(Console.ReadLine() ?? target.Stock.ToString());

                productService.UpdateProduct(target);
                Console.WriteLine("Changes successfully applied across persistence mechanisms.");
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
            }
        }

        static void DisplayLogsFlow()
        {
            Console.Clear();
            Console.WriteLine("=== SECURITY SESSIONS LOG ===");
            foreach (var log in accessLogs)
            {
                Console.WriteLine($"[{log.Timestamp}] User: {log.Username} | Role Context: {log.Role} -> Authorized: {log.Status}");
            }
        }

        static void CheckLowStockAlerts()
        {
            var lowStock = productService.GetLowStockItems();
            if (lowStock.Count > 0)
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
    }
}