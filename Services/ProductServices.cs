using System;
using System.Collections.Generic;
using System.Linq;
using Agullto_IMS.Data;
using Agullto_IMS.Models; // Cleaned up the duplicated/corrupted using statement

namespace Agullto_IMS.Services
{
    public class ProductService
    {
        private readonly InventoryDBData _sqlStorage;
        private readonly JsonInventory _jsonStorage;

        public ProductService(InventoryData data)
        {
            // Null guard for the incoming data configuration
            if (data == null) throw new ArgumentNullException(nameof(data));

            _sqlStorage = new InventoryDBData();
            _jsonStorage = new JsonInventory(data);
        }

        public void AddProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            _sqlStorage.AddProduct(product); // Write to SQLEXPRESS
            _jsonStorage.Add(product);       // Write to Products.json
        }

        public List<Product> GetAllProducts()
        {
            return _sqlStorage.GetProducts() ?? new List<Product>();
        }

        public Product? FindProduct(Guid id)
        {
            return _sqlStorage.GetProducts().FirstOrDefault(p => p.Id == id);
        }

        public bool UpdateProduct(Product product)
        {
            if (product == null) return false;

            bool sqlUpdated = _sqlStorage.UpdateProduct(product);
            bool jsonUpdated = _jsonStorage.Update(product);

            return sqlUpdated && jsonUpdated;
        }

        public bool DeleteProduct(Guid id)
        {
            bool sqlDeleted = _sqlStorage.DeleteProduct(id);
            bool jsonDeleted = _jsonStorage.Delete(id);

            return sqlDeleted && jsonDeleted;
        }

        public List<Product> GetLowStockItems()
        {
            return _sqlStorage.GetProducts()
                .Where(p => p.Stock < 5)
                .ToList();
        }
    }
}