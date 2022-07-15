using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YummyApp.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public List<Ingredient> Ingredients { get; set; }

        public string Content { get; set; }

        public string PreparationTime { get; set; }

        public List<string> Pictures   { get; set; }

        public int Servings { get; set; }

        public List<Review> Reviews { get; set; }

    }
}