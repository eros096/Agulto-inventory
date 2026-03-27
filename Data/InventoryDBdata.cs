using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient; 
using Agullto_IMS.Models;

namespace Agullto_IMS.Data
{
    public class InventoryDBData
    {
        private readonly string _connectionString =
            "Data Source=localhost\\SQLEXPRESS;Initial Catalog=InventoryManagementSystem;Integrated Security=True;TrustServerCertificate=True;";

        
        public List<Product> GetProducts()
        {
            var products = new List<Product>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT Id, Name, Stock, Price FROM Products", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Stock = reader.GetInt32(2),
                            Price = reader.GetDecimal(3)
                        });
                    }
                }
            }

            return products;
        }

        
        public void AddProduct(Product product)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "INSERT INTO Products (Id, Name, Stock, Price) VALUES (@Id, @Name, @Stock, @Price)", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", product.Id);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        
        public bool UpdateProduct(Product product)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "UPDATE Products SET Name=@Name, Stock=@Stock, Price=@Price WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", product.Id);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        
        public bool DeleteProduct(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("DELETE FROM Products WHERE Id=@Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}