using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NextwoIdentity.Data;
using NextwoIdentity.Models;
using System.Data;
using System.Security.Cryptography;

namespace NextwoIdentity.Controllers
{
    public class CategoriesController : Controller
    {
        private NextwoDbContext db;
        public CategoriesController(NextwoDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            return View(db.Categories);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var data = db.Categories.Find(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);

        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Update(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");

            }
            var data = db.Categories.Find(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public IActionResult Create(Category category)
        {
            category.Id=Guid.NewGuid();
            if (ModelState.IsValid)
            {
                if (IsExist(category.Name!))
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Name is already exist!");
                }
            }
            return View(category);
        }
        [HttpGet]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");

            }
            var data = db.Categories.Find(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Delete(Category category)
        {
            var data = db.Categories.Find(category.Id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            db.Categories.Remove(data);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private Boolean IsExist(String catName)
        {
             var result =db.Categories.Any(x=>x.Name==catName);
            if(result) 
                return false;
            return true;
        }
    }
}
