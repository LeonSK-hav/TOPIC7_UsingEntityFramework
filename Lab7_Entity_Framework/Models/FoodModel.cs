using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7_Entity_Framework.Models
{
    public class FoodModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string CategoryName { get; set; } // Changed from int to string
        public int Price { get; set; }
        public string Notes { get; set; }
    }
}
