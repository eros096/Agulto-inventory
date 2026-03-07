using System;
using System.Collections.Generic;
using System.Linq;

namespace Agullto_IMS
{
    internal class Program
    {
        
        static List<Product> products = new List<Product>();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Basic Inventory Management System =====");
                Console.WriteLine("(1. Create) (2. Read) (3. Update) (4. Delete) (5. Exit)");
                Console.Write("Please select an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Enter a number between 1 and 5.");
                    Console.ReadLine();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Create();
                        break;

                    case 2:
                        Read();
                        break;

                    case 3:
                        Update();
                        break;

                    case 4:
                        Delete();
                        break;

                    case 5:
                        Console.WriteLine("Exiting the system. Goodbye!");
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }

        static void Create()
        {
            Console.Write("Enter product name: ");
            string productName = Console.ReadLine();

            Console.Write("Enter product stock: ");
            int productStock;

            while (!int.TryParse(Console.ReadLine(), out productStock))
            {
                Console.Write("Invalid input. Enter a number for stock: ");
            }

            products.Add(new Product
            {
                Name = productName,
                Stock = productStock
            });

            Console.WriteLine("Product added successfully!");
        }

        static void Read()
        {
            Console.WriteLine("\n--- Product List ---");

            if (products.Count == 0)
            {
                Console.WriteLine("No products available.");
            }
            else
            {
                int index = 1;
                foreach (var product in products)
                {
                    Console.WriteLine($"{index}. Name: {product.Name}, Stock: {product.Stock}");
                    index++;
                }
            }
        }

        static void Update()
        {
            Console.Write("Enter product name to update: ");
            string updateProduct = Console.ReadLine();

            var foundProduct = products
                .FirstOrDefault(p => p.Name.Equals(updateProduct, StringComparison.OrdinalIgnoreCase));

            if (foundProduct != null)
            {
                Console.Write("Enter new stock: ");
                int newStock;

                while (!int.TryParse(Console.ReadLine(), out newStock))
                {
                    Console.Write("Invalid input. Enter a number for stock: ");
                }

                foundProduct.Stock = newStock;
                Console.WriteLine("Product updated successfully!");
            }
            else
            {
                Console.WriteLine("Product not found.");
            }
        }

        static void Delete()
        {
            Console.Write("Enter product name to delete: ");
            string deleteProduct = Console.ReadLine();

            var productToDelete = products
                .FirstOrDefault(p => p.Name.Equals(deleteProduct, StringComparison.OrdinalIgnoreCase));

            if (productToDelete != null)
            {
                products.Remove(productToDelete);
                Console.WriteLine("Product deleted successfully!");
            }
            else
            {
                Console.WriteLine("Product not found.");
            }
        }
    }

    class Product
    {
        public string Name { get; set; }
        public int Stock { get; set; }
    }
}