using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
            var recipes_tmp = db.Recipes.Where(r => r.IsPublic).OrderByDescending(r => r.Id).ToList();
            var recipes = recipes_tmp;
            if(recipes_tmp.Count>6)
                recipes = recipes_tmp.GetRange(0, 6);
            var dailyRecipes = db.DailyRecipes.OrderByDescending(x => x.ValidityDate).ToList();
            int dailyRecipeId = -1;
            Recipe recipeOfTheDay = new Recipe() { Author="", Content="",Description="",Title="", PreparationTime="",Servings=0,file=""};
            int index = 0;
            while (true)
            {   
                if (dailyRecipes.Count <= index || dailyRecipes.Count==0)
                    break;

                dailyRecipeId = dailyRecipes.ElementAt(index++).RecipeId;
                var tmpDailyRecipes = db.Recipes.Where(r => r.Id == dailyRecipeId);
                if(tmpDailyRecipes.Count()!=0)
                    
                if (tmpDailyRecipes.First().IsPublic)
                {
                    recipeOfTheDay = tmpDailyRecipes.First();
                    break; 
                }
         
                
            }
            var descendingPostedRecipes = db.Recipes.Where(r => r.IsPublic).OrderByDescending(x => x.Posted);
            var latestRecipe = new Recipe() { Author = "", Content = "", Description = "", Title = "", PreparationTime = "", Servings = 0, file = "" };
            if(descendingPostedRecipes.Count()!=0)
                latestRecipe = descendingPostedRecipes.First();
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
            if (recipe.IsPublic || (recipe.Author==User.Identity.GetUserName() && User.IsInRole("Editor")) || User.IsInRole("Admin"))
            {
                var rnd = new Random();
                return View(new NewRecipeViewModel() { Recipe = recipe, NewReview = new Review(), AllReviews = reviews ,isSaved = saved, 
                    Recommendations = db.Recipes.Include(x=>x.Reviews).ToList().Where(r => r.Type == recipe.Type).OrderBy(x => rnd.Next()).Take(3)});
            }
            else
            {
                return HttpNotFound();
            }
        }


        [Authorize(Roles = "Admin,Editor")]
        public ActionResult RecipeEdit(int Id)
        {
            var recipe = db.Recipes.Find(Id);
            if (recipe != null)
            {
                if (recipe.Author.Equals(User.Identity.GetUserName()) || User.IsInRole("Admin"))
                    return View(recipe);
            }
            return HttpNotFound();
        }
        [Authorize(Roles = "Admin,Editor")]
        public ActionResult RecipeDelete(int Id)
        {

            return View(db.Recipes.Find(Id));
        }
        //[Authorize(Roles = "Admin,Editor")]
        public ActionResult DeleteRecipe(int Id)
        {

            Recipe recipe = db.Recipes.Find(Id);
            if (recipe != null)
            {
                db.Recipes.Remove(recipe);
                db.SaveChanges();
            }
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
        public ActionResult EditRecipe(Recipe model, HttpPostedFileBase file)
        {
            var recipe = db.Recipes.Find(model.Id);
            var file_str = recipe.file;
            try
            {

                //Method 2 Get file details from HttpPostedFileBase class    

                if (file != null)
                {
                    string path = Path.Combine(Server.MapPath("/Images"), model.Id + "-main" + Path.GetExtension(file.FileName));
                    file.SaveAs(path);

                    //recipe.file = "/Images/" + model.Id + "-main" + Path.GetExtension(file.FileName);
                    //recipe.file = path;
                    ViewBag.FileStatus = "File uploaded successfully.";
                }

            }
            catch (Exception)
            {
                ViewBag.FileStatus = "Error while file uploading."; ;
            }
            
            TryUpdateModel(recipe);
            if (file != null)
                recipe.file = "/Images/" + model.Id + "-main" + Path.GetExtension(file.FileName);
            else
                recipe.file = file_str;

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
            return RedirectToAction("RecipeView", "Recipe",new { Id= recipeId });
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
        [Authorize]
        public ActionResult EditReview(int reviewId)
        {
            if (reviewId == null)
                return HttpNotFound("null arg reviewId");
            var review = db.Reviews.Find(reviewId);
            if (review.UserId.Equals(User.Identity.GetUserId()))
                return View(review);
            else
                return HttpNotFound();
        }
        
        public ActionResult ConfirmEditReview(Review model)
        {
            var id = model.Id;
            var review = db.Reviews.Find(id);
            review.Rating = model.Rating;
            review.Content = model.Content;
            db.SaveChanges();

            var recipe = db.Recipes.Find(model.RecipeId);

            return RedirectToAction("RecipeView",new { Id = recipe.Id });
        }
        [Authorize]
        public ActionResult DeleteReview(int reviewId)
        {
            if (reviewId == null)
                return HttpNotFound("null arg reviewId");
            var review = db.Reviews.Find(reviewId);

            if (review.UserId.Equals(User.Identity.GetUserId()))
            {
                var recipeId = review.RecipeId;
                db.Reviews.Remove(review);
                db.SaveChanges();
                return RedirectToAction("RecipeView", new { Id = recipeId });
            }
            else
                return HttpNotFound();
        }
        public ActionResult ShowByCategory(int categoryId)
        {
            var recipes = db.Recipes.Where(x => (int)x.Type == categoryId);
            ViewBag.Title = (Category)categoryId;
            if (categoryId == 0)
                ViewBag.Description = "Welcome to our sweet wonderland. Learn how to make bakery-worthy breads, " +
                                        "cakes, pies, muffins, and scones with our articles. Enjoy these delicious treats " +
                                        "carefully chosen by our experienced team";
            else if (categoryId == 1)
                ViewBag.Description = "Good morning! We are glad you chose our recipes to start your day with. From sweet " +
                                        "to salty treats, you can find everything here.";
            else if (categoryId == 2)
                ViewBag.Description = "Time for lunch! Check out our selection of tasty meals, carefully prepared by " +
                                        "us. We hope you will enjoy preparing the meals as much as we loved creating the recipes.";
            else if (categoryId == 3)
                ViewBag.Description = "Brunch is our favorite daily meal. We love to prepare our brunch and in this section " +
                                    " we would like to share some of our secrets and tricks for perfect brunch. Hope you like them!";
            else if (categoryId == 4)
                ViewBag.Description = "Good evening! Good dinner is a good reward for a productive and challenging day. We hope that " +
                                        "you will accept our dinner suggestions and enjoy your dinner among your loved ones.";
            return View(recipes.ToList());
        }


    }


}