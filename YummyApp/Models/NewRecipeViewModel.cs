using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YummyApp.Models
{
    public class NewRecipeViewModel
    {
        public Recipe Recipe { get; set; }

        public Review NewReview { get; set; }

        public List<Review> AllReviews { get; set; }

        public Boolean isSaved { get; set; }
    }
}