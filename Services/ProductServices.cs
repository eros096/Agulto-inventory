using System;
using System.Linq;
using System.Collections.Generic;
using Agullto_IMS.Models;
using Agullto_IMS.Data;

namespace Agullto_IMS.Services
{
    public class ProductService
    {
        private InventoryData data;

        public ProductService(InventoryData data)
        {
            this.data = data;
        }

        public void AddProduct(string name, int stock)
        {
            data.Products.Add(new Product
            {
                Name = name,
                Stock = stock
            });
        }

        public List<Product> GetProducts()
        {
            return data.Products;
        }

        public bool UpdateProduct(string name, int newStock)
        {
            var product = data.Products
                .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (product == null)
                return false;

            product.Stock = newStock;
            return true;
        }

        public bool DeleteProduct(string name)
        {
            var product = data.Products
                .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (product == null)
                return false;

            data.Products.Remove(product);
            return true;
        }
    }
}