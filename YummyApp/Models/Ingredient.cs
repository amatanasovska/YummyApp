using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YummyApp.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Quantity { get; set; }
        public string Measure { get; set; }
    }
}