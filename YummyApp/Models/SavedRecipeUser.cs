using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YummyApp.Models
{
    public class SavedRecipeUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int RecipeId { get; set; }
    }
}