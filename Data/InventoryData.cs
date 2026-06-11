using System.Collections.Generic;
using Agullto_IMS.Models;

namespace Agullto_IMS.Data
{
    public class InventoryData
    {
        // Changed from a public field to a public property
        public List<Product> Products { get; set; } = new List<Product>();
    }
}