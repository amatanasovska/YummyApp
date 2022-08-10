using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YummyApp.Models
{
    public class RecipeViewModel
    {
        public Recipe RecipeOfTheDay { get; set; }
        public Recipe LatestRecipe { get; set; }
        public Recipe HighestRatingRecipe { get; set; }
        public IEnumerable<Recipe> Recipes { get; set; }


    }
}