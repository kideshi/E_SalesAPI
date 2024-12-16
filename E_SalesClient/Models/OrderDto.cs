using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_SalesClient.Models
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string Product { get; set; }
        public string Customer { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
