using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YummyApp.Models;

namespace YummyApp.Controllers
{
    public class RecipeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Recipe
        public ActionResult Index()
        {
            var recipes = db.Recipes.ToList();
            return View(recipes);
        }
    }
}