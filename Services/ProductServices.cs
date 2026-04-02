using System;
using System.Collections.Generic;
using System.Linq;
using Agullto_IMS.Models;
using Agullto_IMS.Data;

namespace Agullto_IMS.Services
{
    public class ProductService
    {
        private readonly InventoryData _data;
        private readonly InventoryDBData _dbData;

        public ProductService(InventoryData data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _dbData = new InventoryDBData(); 
        }

        
        public Product AddProduct(string name, int stock, decimal price)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Stock = stock,
                Price = price
            };

            _data.Products.Add(product);
            _dbData.AddProduct(product);

            return product;
        }

        
        public List<Product> GetProducts()
        {
            return _data.Products;
        }

        public bool UpdateProduct(Guid id, string newName, int newStock, decimal newPrice)
        {
            var product = _data.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return false;

            product.Name = newName;
            product.Stock = newStock;
            product.Price = newPrice;

            return _dbData.UpdateProduct(product);
        }

        
        public bool DeleteProduct(Guid id)
        {
            var product = _data.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return false;

            _data.Products.Remove(product);
            return _dbData.DeleteProduct(id);
        }
    }
}