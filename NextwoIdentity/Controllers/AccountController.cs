using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NextwoIdentity.Data;
using NextwoIdentity.Models;
using NextwoIdentity.Models.ViewModels;
using System.Diagnostics;

namespace NextwoIdentity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Configuration
        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;
        private RoleManager<IdentityRole> roleManager;
        private NextwoDbContext db;


        public AccountController(UserManager<IdentityUser> _userManager,
            SignInManager<IdentityUser> _signInManager,
            RoleManager<IdentityRole> _roleManager,
            NextwoDbContext _db)
        {
            userManager= _userManager;
            signInManager= _signInManager;
            roleManager=_roleManager;
            db= _db;
        }
        #endregion


        #region Users
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = model.Email,
                    Email= model.Email,
                    PhoneNumber=model.Phone
                };
                var result = await userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);
            }

            return View(model);
        }

        [AllowAnonymous]

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync
                    (model.Email!, model.Password!, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid user name or password");
                return View(model); 
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
        #endregion

        #region Roles
       // [Authorize(Roles ="Admin")]
        public IActionResult CreateRole()
        {
           
            return View();  
        }
        [HttpPost]
     //   [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(CreteRoleViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole
                {
                    Name=model.RoleName
                };
                var result=await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }
                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);
            }
            return View(model); 
        }
        [Authorize(Roles = "Admin")]

        public IActionResult RolesList()
        {
            return View(roleManager.Roles);
        }


        /*public IActionResult AccessDeny()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("AccessDeny");
            }
            return View();
        }*/
        public IActionResult AccessDenied()
        {
            return View();
        }
       
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {

            if(id== null)
            {
                return RedirectToAction("RolesList");

            }
            var role=await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToAction("RolesList");
            }
            EditRoleViewModel model=new EditRoleViewModel
            {
                RoleName = role.Name,
                RoleId= role.Id,
            };
            foreach(var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user,role.Name!))
                {
                    model.Users!.Add(user.UserName!);
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(model.RoleId!);
                if (role == null)
                {
                    return RedirectToAction(nameof(ErrorPage));
                }
                role.Name=model.RoleName;
                var result=await roleManager.UpdateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction(nameof(RolesList));
                }
                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);

            }
            return View(model);
        }
        public IActionResult ErrorPage()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ModifyUserInRole(string id)
        {
            if (id==null)
            {
                return RedirectToAction(nameof(RolesList));
            }
            var role= await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return RedirectToAction(nameof(ErrorPage));
            }
            List<UserRoleViewModel> models= new List<UserRoleViewModel>();
            foreach(var user in userManager.Users)
            {
                UserRoleViewModel userRole= new UserRoleViewModel
                {
                    UserId= user.Id,
                    UserName= user.UserName,
                };
                if (await userManager.IsInRoleAsync(user, role.Name!))
                {
                    userRole.IsSelected= true;
                }
                else
                {
                    userRole.IsSelected=false;
                }
                models.Add(userRole);   
            }
            return View(models);
        }
        [HttpPost]
        public async Task<IActionResult> ModifyUsersInRole(string id, List<UserRoleViewModel> models)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(RolesList));
            }
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToAction(nameof(ErrorPage));
            }
            IdentityResult result = new IdentityResult();
            for (int i = 0; i < models.Count; i++)
            {

                var user = await userManager.FindByIdAsync(models[i].UserId!);
                if (models[i].IsSelected && (!await userManager.IsInRoleAsync(user!, role.Name!)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name!);
                }
                else if (!models[i].IsSelected && (await userManager.IsInRoleAsync(user!, role.Name!)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name!);
                }

            }
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(RolesList));
            }
            return View(models);

        }




        #endregion

        #region Products
        [HttpGet]
        
         [Authorize(Roles ="Admin")]
        public IActionResult CreteProduct()
        {
            ViewBag.CategoryId=new SelectList(db.Categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public IActionResult CreteProduct(Product product)
        {
            product.Id=Guid.NewGuid();
            if (ModelState.IsValid)
            {

                db.Products.Add(product);
                db.SaveChanges();
               return RedirectToAction("AllProducts");

            }
            return View(product);
        }

        [AllowAnonymous]
        public IActionResult AllProducts() {
            return View(db.Products.Include(x=>x.Category));
        }
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public IActionResult EditProduct(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("AllProducts");

            }
            var data = db.Products.Find(id);
            if (data == null)
            {
                return RedirectToAction("AllProducts");
            }
            return View(data);
            
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Update(product);
                db.SaveChanges();
                return RedirectToAction("AllProducts");
            }
            return View(product);
        }
        [HttpGet]
        [Authorize(Roles ="User")]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("AllProducts");

            }
            var data = db.Products.Find(id);
            if (data == null)
            {
                return RedirectToAction("AllProducts");
            }
            return View(data);
        }
        [HttpGet]
        [Authorize(Roles ="Admin")]

        public IActionResult CreateCategory()
        {
           
            return View();
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]

        public IActionResult CreateCategory(Category category)
        {
            category.Id=Guid.NewGuid();
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
              return  RedirectToAction("AllCategories");
            }
            return View(category);
        }
        public IActionResult Delete(Guid? id)
        {
            return View();
        }
        #endregion
    }

}
