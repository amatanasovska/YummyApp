﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YummyApp.Models
{
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

        public DateTime Posted { get; set; }
        public float Average()
        {
            if (Reviews == null)
            {
                return 0.0f;
            }

            var total = 0.0f;
                
                
            foreach (Review review in Reviews)
            {
                total = total + review.Rating;
            }
                    
                
                total = total/Reviews.Count;
                
            return total;
        }
    }
}