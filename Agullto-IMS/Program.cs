using System;
using Agullto_IMS.Services;
using Agullto_IMS.Data;

namespace Agullto_IMS
{
    class Program
    {
        static void Main(string[] args)
        {
            InventoryData data = new InventoryData();
            ProductService service = new ProductService(data);

            while (true)
            {
                
                Console.Clear();
                Console.WriteLine("===== Basic Inventory Management System =====");
                Console.WriteLine("(1. Create) (2. Read) (3. Update) (4. Delete) (5. Exit)");
                Console.Write("Please select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input.");
                    Console.ReadLine();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter product name: ");
                        string name = Console.ReadLine() ?? "";

                        Console.Write("Enter stock: ");
                        int stock;
                        while (!int.TryParse(Console.ReadLine(), out stock))
                        {
                            Console.Write("Enter a valid number: ");
                        }

                        service.AddProduct(name, stock);
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
                                Console.WriteLine($"{i}. {p.Name} - {p.Stock}");
                                i++;
                            }
                        }
                        break;

                    case 3:
                        Console.Write("Enter product name to update: ");
                        string updateName = Console.ReadLine() ?? "";

                        Console.Write("Enter new stock: ");
                        int newStock;

                        while (!int.TryParse(Console.ReadLine(), out newStock))
                        {
                            Console.Write("Enter a valid number: ");
                        }

                        if (service.UpdateProduct(updateName, newStock))
                            Console.WriteLine("Product updated!");
                        else
                            Console.WriteLine("Product not found.");
                        break;

                    case 4:
                        Console.Write("Enter product name to delete: ");
                        string deleteName = Console.ReadLine() ?? "";

                        if (service.DeleteProduct(deleteName))
                            Console.WriteLine("Product deleted!");
                        else
                            Console.WriteLine("Product not found.");
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
    }
}