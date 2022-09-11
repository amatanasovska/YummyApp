using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YummyApp.Models
{
    public enum Category
    {
        Dessert, Breakfast, Lunch, Brunch, Dinner
    }
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }



        [Required]
        [MaxLength(100)]
        public string Description { get; set; }


        public List<Ingredient> Ingredients { get; set; }

        [Required]
        [AllowHtml]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Preparation Time")]
        public string PreparationTime { get; set; }

        public Picture picture { get; set; }
        public List<Picture> Pictures { get; set; }

        [Required]
        public int Servings { get; set; }

        public List<Review> Reviews { get; set; }
        public string Author { get; set; }
        [Display(Name = "Publish recipe")]
        public bool IsPublic { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Upload File")]
        public string file { get; set; }
        public DateTime Posted { get; set; }
        public float Average()
        {
            if (Reviews == null || Reviews.Count==0)
            {
                return (float)0.0;
            }

            var total = 0.0f;
                
                
            foreach (Review review in Reviews)
            {
                total = total + review.Rating;
            }
                    
                
                total = total/Reviews.Count;

              return total;

        }

        public Category Type { get; set; }
    }
}