using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Agullto_IMS.Models;

namespace Agullto_IMS.Data
{
    public class InventoryDBData
    {
        private readonly string _connectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=InventoryManagementSystem;Integrated Security=True;TrustServerCertificate=True;";

        public List<Product> GetProducts()
        {
            var products = new List<Product>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT Id, Name, Stock, Department, WeightValue, Unit, 
                                        CostPrice, SellingPrice, Location, ExpirationDate 
                                 FROM Products";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Stock = reader.GetInt32(2),
                            // Safe casting from Database Int back to your custom C# Enums
                            Department = (ProductDepartment)reader.GetInt32(3),
                            WeightValue = reader.GetDouble(4),
                            Unit = (MeasurementUnit)reader.GetInt32(5),
                            CostPrice = reader.GetDecimal(6),
                            SellingPrice = reader.GetDecimal(7),
                            // Checked safe null extractions for empty warehouse layout markers
                            Location = reader.IsDBNull(8) ? null : reader.GetString(8),
                            ExpirationDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9)
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
                string query = @"INSERT INTO Products (Id, Name, Stock, Department, WeightValue, Unit, CostPrice, SellingPrice, Location, ExpirationDate) 
                                 VALUES (@Id, @Name, @Stock, @Department, @WeightValue, @Unit, @CostPrice, @SellingPrice, @Location, @ExpirationDate)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", product.Id);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);
                    cmd.Parameters.AddWithValue("@Department", (int)product.Department);
                    cmd.Parameters.AddWithValue("@WeightValue", product.WeightValue);
                    cmd.Parameters.AddWithValue("@Unit", (int)product.Unit);
                    cmd.Parameters.AddWithValue("@CostPrice", product.CostPrice);
                    cmd.Parameters.AddWithValue("@SellingPrice", product.SellingPrice);
                    cmd.Parameters.AddWithValue("@Location", (object)product.Location ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExpirationDate", (object)product.ExpirationDate ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool UpdateProduct(Product product)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"UPDATE Products 
                                 SET Name=@Name, Stock=@Stock, Department=@Department, WeightValue=@WeightValue, 
                                     Unit=@Unit, CostPrice=@CostPrice, SellingPrice=@SellingPrice, 
                                     Location=@Location, ExpirationDate=@ExpirationDate 
                                 WHERE Id=@Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", product.Id);
                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);
                    cmd.Parameters.AddWithValue("@Department", (int)product.Department);
                    cmd.Parameters.AddWithValue("@WeightValue", product.WeightValue);
                    cmd.Parameters.AddWithValue("@Unit", (int)product.Unit);
                    cmd.Parameters.AddWithValue("@CostPrice", product.CostPrice);
                    cmd.Parameters.AddWithValue("@SellingPrice", product.SellingPrice);
                    cmd.Parameters.AddWithValue("@Location", (object)product.Location ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExpirationDate", (object)product.ExpirationDate ?? DBNull.Value);

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