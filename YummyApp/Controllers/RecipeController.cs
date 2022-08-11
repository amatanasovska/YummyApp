﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
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
            var recipes = db.Recipes.Where(r => r.IsPublic).ToList();
            var dailyRecipeId = db.DailyRecipes.OrderByDescending(x => x.ValidityDate).First().RecipeId;
            var recipeOfTheDay = db.Recipes.Where(r => r.Id == dailyRecipeId).First();
            var latestRecipe = db.Recipes.Where(r => r.IsPublic).OrderByDescending(x => x.Posted).First();
            var highest_review_rating = 0.0;
           List<Recipe> highestRatingRecipes = db.Recipes.Include("Reviews").Where(r=>r.IsPublic).ToList();
            var highestRatingRecipe = recipeOfTheDay;
            foreach (Recipe recipe in highestRatingRecipes)
            {
                var total = 0.0;
                if (recipe.Reviews != null)
                {
                    foreach (Review review in recipe.Reviews.ToList())
                    {
                        total = total + review.Rating;
                    }
                    
                
                total = total/recipe.Reviews.ToList().Count;
                }
                if (recipe.Average() >= highest_review_rating)
                {
                    highestRatingRecipe = recipe;
                    highest_review_rating = recipe.Average();
                }
            }

            RecipeViewModel rvm = new RecipeViewModel() {HighestRatingRecipe = highestRatingRecipe ,RecipeOfTheDay = recipeOfTheDay, LatestRecipe = latestRecipe,Recipes = recipes};
            return View(rvm);
        }
        
        public ActionResult RecipeView(int Id)
        {
            var reviews = db.Reviews.Where(x => x.RecipeId == Id).ToList();
            var recipe = db.Recipes.Find(Id);
            var userId = User.Identity.GetUserId();
            var savedEntry = db.SavedRecipeUser.Where(x => x.RecipeId.Equals(Id) && x.UserId.Equals(userId)).ToList();
            bool saved = true;
            if(savedEntry.Count==0)
            {
                saved = false;
            }
            if (recipe.IsPublic || (recipe.Author==User.Identity.GetUserId() && User.IsInRole("Editor")) || User.IsInRole("Admin"))
            {
                return View(new NewRecipeViewModel() { Recipe = recipe, NewReview = new Review(), AllReviews = reviews ,isSaved = saved});
            }
            else
            {
                return HttpNotFound();
            }
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
        public ActionResult RemoveSavedRecipe( int Id, string UserId)
        {

            SavedRecipeUser recipeuser = db.SavedRecipeUser.Where(r => r.RecipeId==Id && r.UserId.Equals(UserId)).First();
            db.SavedRecipeUser.Remove(recipeuser);
            db.SaveChanges();
            return RedirectToAction("SavedRecipes", "Account");
        }
        [Authorize]
        public ActionResult EditRecipe(Recipe model)
        {
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListRecipes", "Account");
        }
        [Authorize]
        public ActionResult SaveRecipe(int recipeId)
        {

            var userId = User.Identity.GetUserId();
            var user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            var recipe = db.Recipes.Find(recipeId);
            if(db.SavedRecipeUser.Where(r => r.RecipeId==recipeId && r.UserId.Equals(userId)).Count()!=0)
            {
                return RedirectToAction("RecipeView", recipe);
            }
            db.SavedRecipeUser.Add(new SavedRecipeUser() { RecipeId = recipeId, UserId = userId});
            
            db.SaveChanges();
            return RedirectToAction("RecipeView", recipe);
        }
        [Authorize]
        public ActionResult CreateReview(NewRecipeViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var UserReviews = db.Reviews.Where(review => review.UserId.Equals(userId) && review.RecipeId.Equals(model.Recipe.Id)).ToList();
            if (UserReviews.Count == 0)
            {
                var recipe = db.Recipes.Find(model.Recipe.Id);

                if (recipe.Reviews != null && recipe.Reviews.Where(x => x.UserId.Equals(User.Identity.GetUserId())).ToList().Count != 0)
                {
                    return HttpNotFound();
                }


                Review newReview = new Review() { RecipeId = recipe.Id, Content = model.NewReview.Content, Rating = model.NewReview.Rating, UserId = User.Identity.GetUserId() };
                if (recipe.Reviews == null)
                {
                    recipe.Reviews = new List<Review>();
                }
                if (recipe.Reviews.Where(x => x.UserId.Equals(User.Identity.GetUserId())).ToList().Count == 0)
                {
                    recipe.Reviews.Add(newReview);
                    db.SaveChanges();
                    return RedirectToAction("RecipeView", "Recipe", new { Id = recipe.Id });

                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                return HttpNotFound();
            }
        }
   
        public ActionResult SearchRecipe(string model)
        {
            var Recipes = db.Recipes.ToList();
            if (model == "" || model == null)
            {
                return View(Recipes);
            }
            else
            {
                var result = Recipes.Where(n => Regex.IsMatch(n.Title, Regex.Escape(model), RegexOptions.IgnoreCase) || Regex.IsMatch(n.Description, Regex.Escape(model), RegexOptions.IgnoreCase)).ToList();
                return View(result);
            }
        }
    }

}