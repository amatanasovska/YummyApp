using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using YummyApp.Models;

namespace YummyApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly UserManager<IdentityUser> _userManagerIU;
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    UserManager.AddToRole(user.Id, "StandardUser");
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Recipe");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        public async Task<ActionResult> SiteSettings()
        {
            var latestRecipe = db.DailyRecipes.OrderByDescending(x => x.ValidityDate).First();
            var latestDate = latestRecipe.ValidityDate;
            var isToday = true;

            var Editors = (from u in db.Users where u.Roles.Any(r => r.RoleId == "1" && r.RoleId!="0")
                           select u).ToList();
            var numberOfEditors = Editors.Count();
            if (!latestDate.ToString("MM/dd/yyyy").Equals(DateTime.Now.ToString("MM/dd/yyyy")))
            {
                isToday = false;
            }
            SiteSettingsViewModel ssvm = new SiteSettingsViewModel() { isDayRecipeSet = isToday, DayEditorId = Editors.ElementAt((latestRecipe.Id+1)%numberOfEditors).Id};
            return View(ssvm);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult AddUserToRole()
        {
            AddToRoleModel model = new AddToRoleModel();
            model.roles = new List<string>() { "Admin", "Editor", "StandardUser" };
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AddUserToRole(AddToRoleModel model)
        {
            var email = model.Email;
            var user = UserManager.FindByEmail(model.Email);
            if (user == null)
                throw new HttpException(404, "There is no user with email: " + model.Email);

            UserManager.AddToRole(user.Id, model.selectedRole);
            return RedirectToAction("Index", "Recipe");
        }
        [Authorize(Roles = "Admin,Editor")]
        public ActionResult ListRecipes()
        {
            List<Recipe> recipes = null;
             if(User.IsInRole("Editor"))
            {   
             recipes = db.Recipes.Include("Reviews").Where(x => x.Author.Equals(User.Identity.Name)).ToList();
            } 
             else if(User.IsInRole("Admin"))
            {
                recipes = db.Recipes.Include("Reviews").ToList();
            }
            return View(recipes);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ManageEditors()
        {
            List<ApplicationUser> users = (from u in db.Users
        where u.Roles.Any(r => r.RoleId == "1")
        select u).ToList();
            return View(users);
        }
        [Authorize(Roles = "Admin,Editor")]
        public ActionResult AddNewRecipe()
        {
            return View(new Recipe() {Reviews = new List<Review>(), Ingredients = new List<Ingredient>(), Pictures = new List<Picture>(),Posted = DateTime.Now }) ;
        }
        [Authorize(Roles = "Admin,Editor")]
        public ActionResult CreateRecipe(Recipe model)
        {
            db.Recipes.Add(model);
            model.Author = User.Identity.GetUserName();
            model.file = "/Images/no-image.bmp";
            db.SaveChanges();
            return View("UploadRecipeImage", model);
        }
        [Authorize]
        public ActionResult SavedRecipes()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            var SavedRecipeUser = db.SavedRecipeUser.Where(u => u.UserId.Equals(userId)).ToList();
            var SavedRecipes = new List<Recipe>();
            foreach(SavedRecipeUser s in SavedRecipeUser)
            {
                SavedRecipes.Add(db.Recipes.Find(s.RecipeId));
            }
            return View(SavedRecipes); 
        }
        public ActionResult UploadRecipeImage(Recipe recipe)
        {
            return View(recipe);
        }
        [HttpPost]
        public ActionResult UploadFiles(Recipe model,HttpPostedFileBase file)
        {
            
                try
                {

                    //Method 2 Get file details from HttpPostedFileBase class    

                    if (file != null)
                    {
                        string path = Path.Combine(Server.MapPath("/Images"), model.Id+"-main"+Path.GetExtension(file.FileName));
                        file.SaveAs(path);
                    
                        db.Recipes.Find(model.Id).file = "/Images/" + model.Id + "-main"+Path.GetExtension(file.FileName);
                        db.SaveChanges();
                        ViewBag.FileStatus = "File uploaded successfully."; 
                    }
                    else
                    {
                    string path = Path.Combine(Server.MapPath("/Images"), "no-image.bmp");
                    file.SaveAs(path);

                    db.Recipes.Find(model.Id).file = "/Images/" + model.Id + "-main" + Path.GetExtension(file.FileName);
                    db.SaveChanges();
                    ViewBag.FileStatus = "File uploaded successfully.";
                }
                
                }
                catch (Exception)
                {
                    ViewBag.FileStatus = "Error while file uploading."; ;
                }
            
            return View("UploadRecipeImage",model);
        }
        public ActionResult ListEditorPosts(string Id)
        {
            return View(db.Recipes.Where(r => r.Author == db.Users.FirstOrDefault(u => u.Id == Id).UserName).ToList());
        }
        public ActionResult RemoveEditorRole(string Id)
        {
            
            UserManager.RemoveFromRole(Id, "Editor");

            return RedirectToAction("ManageEditors");
        }
        public ActionResult SelectDayRecipe()
        {
            List<Recipe> recipes = null;
            if (User.IsInRole("Editor"))
            {
                recipes = db.Recipes.Where(x => x.Author.Equals(User.Identity.Name)).ToList();
            }
            else if (User.IsInRole("Admin"))
            {
                recipes = db.Recipes.ToList();
            }
            return View(recipes);
        }
        public ActionResult ChooseDayRecipe(int Id)
        {
            var latestRecipe = db.DailyRecipes.OrderByDescending(x => x.ValidityDate).First();
            var latestDate = latestRecipe.ValidityDate;
            var isToday = true;

            var Editors = (from u in db.Users
                           where u.Roles.Any(r => r.RoleId == "1" && r.RoleId != "0")
                           select u).ToList();
            var numberOfEditors = Editors.Count();
            if (!latestDate.ToString("MM/dd/yyyy").Equals(DateTime.Now.ToString("MM/dd/yyyy")))
            {
                isToday = false;
            }
            if(User.IsInRole("Admin") || (!isToday && User.Identity.GetUserId() == Editors.ElementAt((latestRecipe.Id + 1) % numberOfEditors).Id))
            {
                var recipe = db.Recipes.Where(x => x.Id == Id).Single();
                if (recipe.IsPublic)
                {
                    db.DailyRecipes.Add(new DailyRecipe() { RecipeId = recipe.Id, ValidityDate = DateTime.Now });
                    db.SaveChanges();
                }
                return RedirectToAction("SiteSettings", "Account");
            }
            else if (User.Identity.GetUserId() != Editors.ElementAt((latestRecipe.Id + 1) % numberOfEditors).Id)
            {
                return HttpNotFound();
            }
            else
            {
                return View("ErrorAlreadyDone");
            }
        }
        public ActionResult UserReviews()
        {
            Dictionary<string, Review> dictionary = new Dictionary<string, Review>();
            var userId = User.Identity.GetUserId();
            var reviews = db.Reviews.Where(r => r.UserId.Equals(userId)).ToList();
            foreach(Review r in reviews)
            {
                var recipeName = db.Recipes.Find(r.RecipeId).Title;
                dictionary[recipeName] = r;
            }
            return View(new UserReviewViewModel() { Reviews = dictionary });
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Recipe");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Recipe");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}