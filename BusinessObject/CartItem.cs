using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class CartItem
    {
        public int quantity { set; get; }
        public ProductObject product { set; get; }
    }
}
