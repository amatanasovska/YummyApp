using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public ActionResult RecipeView(int Id)
        {
            return View(db.Recipes.Find(Id));
        }
        public ActionResult RecipeEdit(int Id)
        {
            return View(db.Recipes.Find(Id));
        }
        public ActionResult RecipeDelete(int Id)
        {

            return View(db.Recipes.Find(Id));
        }
        public ActionResult DeleteRecipe(Recipe model, int Id)
        {

            Recipe recipe = db.Recipes.Find(Id);
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("ListRecipes", "Account");
        }
        public ActionResult EditRecipe(Recipe model)
        {
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListRecipes", "Account");
        }
    }

}