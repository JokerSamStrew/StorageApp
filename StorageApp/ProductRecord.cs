using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageApp
{
    public class ProductRecord
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
        public DateTime Date { get; set; }

        public string CurrentStatus { get; set; }
    }
}
