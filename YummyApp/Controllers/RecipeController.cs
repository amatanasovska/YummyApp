using Microsoft.AspNet.Identity;
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
            var dailyRecipeId = db.DailyRecipes.OrderByDescending(x => x.ValidityDate).First().RecipeId;
            var recipeOfTheDay = db.Recipes.Where(r => r.Id == dailyRecipeId).First();
            var latestRecipe = db.Recipes.OrderByDescending(x => x.Posted).First();
            RecipeViewModel rvm = new RecipeViewModel() { RecipeOfTheDay = recipeOfTheDay, LatestRecipe = latestRecipe,Recipes = recipes};
            return View(rvm);
        }
        
        public ActionResult RecipeView(int Id)
        {
            var reviews = db.Reviews.Where(x => x.RecipeId == Id).ToList();
            return View(new NewRecipeViewModel() { Recipe = db.Recipes.Find(Id), NewReview = new Review() , AllReviews = reviews });
        }
        [Authorize]
        public ActionResult RecipeEdit(int Id)
        {
            return View(db.Recipes.Find(Id));
        }
        [Authorize]
        public ActionResult RecipeDelete(int Id)
        {

            return View(db.Recipes.Find(Id));
        }
        [Authorize]
        public ActionResult DeleteRecipe(Recipe model, int Id)
        {

            Recipe recipe = db.Recipes.Find(Id);
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("ListRecipes", "Account");
        }
        [Authorize]
        public ActionResult EditRecipe(Recipe model)
        {
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListRecipes", "Account");
        }
        [Authorize]
        public ActionResult CreateReview(NewRecipeViewModel model)
        {

            var recipe = db.Recipes.Find(model.Recipe.Id);

            Review newReview = new Review() { RecipeId=recipe.Id, Content = model.NewReview.Content, Rating = model.NewReview.Rating, UserId = User.Identity.GetUserId() };
            if(recipe.Reviews==null)
            {
                recipe.Reviews = new List<Review>();
            }
            recipe.Reviews.Add(newReview);
            db.SaveChanges();
            return View("RecipeView", new NewRecipeViewModel() { Recipe = recipe, NewReview = new Review(), AllReviews = db.Reviews.Where(x => x.RecipeId == recipe.Id).ToList() });
        }
    }

}