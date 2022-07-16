using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YummyApp.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }

        
        public List<Ingredient> Ingredients { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string PreparationTime { get; set; }
        
       
        public List<string> Pictures   { get; set; }
        
        [Required]
        public int Servings { get; set; }

        public List<Review> Reviews { get; set; }

    }
}