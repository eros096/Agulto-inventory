using System;

namespace Agullto_IMS.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public int Stock { get; set; }
        public ProductDepartment Department { get; set; }
        public double WeightValue { get; set; }
        public MeasurementUnit Unit { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string? Location { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    public enum ProductDepartment { Produce, Meat, Dairy, Bakery, Frozen, Pantry, Beverages }
    public enum MeasurementUnit { Pcs, Kg, G, L, Ml, Pack, Box }

    public class UserAccount
    {
        public Guid AccountId { get; set; } = Guid.NewGuid();
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; } // "Admin" or "Employee"
    }

    public class AccessLog
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}