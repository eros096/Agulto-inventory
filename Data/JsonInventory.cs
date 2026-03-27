using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Agullto_IMS.Models;

namespace Agullto_IMS.Data
{
    public class JsonInventory
    {
        private readonly InventoryData _inventoryData;
        private readonly string _jsonFileName;

        public JsonInventory(InventoryData inventoryData)
        {
            _inventoryData = inventoryData ?? throw new ArgumentNullException(nameof(inventoryData));
            _jsonFileName = $"{AppDomain.CurrentDomain.BaseDirectory}/Products.json";

            PopulateJsonFile();
        }

        public void PopulateJsonFile()
        {
            RetrieveDataFromJsonFile();

            if (_inventoryData.Products.Count == 0)
            {
                _inventoryData.Products.AddRange(new[]
                {
                    new Product { Id = Guid.NewGuid(), Name = "Laptop", Stock = 10, Price = 45000 },
                    new Product { Id = Guid.NewGuid(), Name = "Mouse", Stock = 50, Price = 500 },
                    new Product { Id = Guid.NewGuid(), Name = "Keyboard", Stock = 30, Price = 1500 }
                });

                SaveDataToJsonFile();
            }
        }

        public void RetrieveDataFromJsonFile()
        {
            if (!File.Exists(_jsonFileName))
            {
                _inventoryData.Products.Clear();
                return;
            }

            var json = File.ReadAllText(_jsonFileName);
            var productsFromFile = JsonSerializer.Deserialize<Product[]>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? Array.Empty<Product>();

            _inventoryData.Products.Clear();
            _inventoryData.Products.AddRange(productsFromFile);
        }

        public void AddProduct(Product product)
        {
            if (product.Id == Guid.Empty)
                product.Id = Guid.NewGuid();

            _inventoryData.Products.Add(product);
            SaveDataToJsonFile();
        }

        public void RemoveProduct(Guid id)
        {
            var product = _inventoryData.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _inventoryData.Products.Remove(product);
                SaveDataToJsonFile();
            }
        }

        public void SaveDataToJsonFile()
        {
            var json = JsonSerializer.Serialize(_inventoryData.Products, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_jsonFileName, json);
        }
    }
}