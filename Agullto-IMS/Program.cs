using System;
using Agullto_IMS.Services;
using Agullto_IMS.Data;
using Agullto_IMS.Models;


namespace Agullto_IMS
{
    

    

    class Program
    {
        static void Main(string[] args)
        {
            InventoryData inventory = new InventoryData();
            JsonInventory jsonInventory = new JsonInventory(inventory); 
            ProductService service = new ProductService(inventory);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Basic Inventory Management System =====");
                Console.WriteLine("(1. Create) (2. Read) (3. Update) (4. Delete) (5. Exit)");
                Console.Write("Please select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Press Enter to continue...");
                    Console.ReadLine();
                    continue;
                }

                switch (choice)
                {
                    case 1: 
                        Console.Write("Enter product name: ");
                        string name = Console.ReadLine() ?? "";

                        int stock = ReadInt("Enter stock: ");
                        decimal price = ReadDecimal("Enter price: ");

                        var newProduct = service.AddProduct(name, stock, price);
                        jsonInventory.AddProduct(newProduct);  
                        Console.WriteLine("Product added!");
                        break;

                    case 2: 
                        var products = service.GetProducts();
                        if (products.Count == 0)
                        {
                            Console.WriteLine("No products available.");
                        }
                        else
                        {
                            int i = 1;
                            foreach (var p in products)
                            {
                                Console.WriteLine($"{i}. ID: {p.Id} | {p.Name} - {p.Stock} pcs - {p.Price:C}");
                                i++;
                            }
                        }
                        break;

                    case 3: 
                        products = service.GetProducts();
                        if (products.Count == 0)
                        {
                            Console.WriteLine("No products available.");
                            break;
                        }

                        DisplayProducts(products);

                        Console.Write("Enter the ID of the product to update: ");
                        if (!Guid.TryParse(Console.ReadLine(), out Guid updateId))
                        {
                            Console.WriteLine("Invalid ID.");
                            break;
                        }

                        Console.Write("Enter new name: ");
                        string newName = Console.ReadLine() ?? "";
                        int newStock = ReadInt("Enter new stock: ");
                        decimal newPrice = ReadDecimal("Enter new price: ");

                        if (service.UpdateProduct(updateId, newName, newStock, newPrice))
                        {
                            jsonInventory.SaveDataToJsonFile(); 
                            Console.WriteLine("Product updated!");
                        }
                        else
                        {
                            Console.WriteLine("Product not found.");
                        }
                        break;

                    case 4: 
                        products = service.GetProducts();
                        if (products.Count == 0)
                        {
                            Console.WriteLine("No products available.");
                            break;
                        }

                        DisplayProducts(products);

                        Console.Write("Enter the ID of the product to delete: ");
                        if (!Guid.TryParse(Console.ReadLine(), out Guid deleteId))
                        {
                            Console.WriteLine("Invalid ID.");
                            break;
                        }

                        if (service.DeleteProduct(deleteId))
                        {
                            jsonInventory.SaveDataToJsonFile(); 
                            Console.WriteLine("Product deleted!");
                        }
                        else
                        {
                            Console.WriteLine("Product not found.");
                        }
                        break;

                    case 5: 
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }

        
        static void DisplayProducts(System.Collections.Generic.List<Product> products)
        {
            int i = 1;
            foreach (var p in products)
            {
                Console.WriteLine($"{i}. ID: {p.Id} | {p.Name} - {p.Stock} pcs - {p.Price:C}");
                i++;
            }
        }

        
        static int ReadInt(string prompt)
        {
            int value;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Enter a valid number: ");
            }
            return value;
        }

        
        static decimal ReadDecimal(string prompt)
        {
            decimal value;
            Console.Write(prompt);
            while (!decimal.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Enter a valid decimal: ");
            }
            return value;
        }
    }
}