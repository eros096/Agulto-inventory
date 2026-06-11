using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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

            RetrieveDataFromJsonFile();
        }

        public List<Product> GetAll()
        {
            return _inventoryData.Products;
        }

        public void Add(Product product)
        {
            if (product.Id == Guid.Empty)
                product.Id = Guid.NewGuid();

            _inventoryData.Products.Add(product);
            SaveDataToJsonFile();
        }

        public bool Update(Product updatedProduct)
        {
            var existingProduct = _inventoryData.Products.FirstOrDefault(p => p.Id == updatedProduct.Id);
            if (existingProduct == null)
                return false;

            // Mapping all advanced features to the JSON storage state
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Stock = updatedProduct.Stock;
            existingProduct.Department = updatedProduct.Department;
            existingProduct.WeightValue = updatedProduct.WeightValue;
            existingProduct.Unit = updatedProduct.Unit;
            existingProduct.CostPrice = updatedProduct.CostPrice;
            existingProduct.SellingPrice = updatedProduct.SellingPrice;
            existingProduct.Location = updatedProduct.Location;
            existingProduct.ExpirationDate = updatedProduct.ExpirationDate;

            SaveDataToJsonFile();
            return true;
        }

        public bool Delete(Guid id)
        {
            var product = _inventoryData.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return false;

            _inventoryData.Products.Remove(product);
            SaveDataToJsonFile();
            return true;
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

        public void SaveDataToJsonFile()
        {
            var json = JsonSerializer.Serialize(_inventoryData.Products, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_jsonFileName, json);
        }
    }
}