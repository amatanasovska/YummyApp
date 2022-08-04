using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YummyApp.Models
{
    public class DailyRecipe
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }

        public DateTime ValidityDate { get; set; }
    }
}