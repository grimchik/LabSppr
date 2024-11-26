using Bar.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bar.Domain.Models
{
    public class CartItem
    {
        public Product Item { get; set; } = null!;
        public int Count { get; set; }
    }
}
