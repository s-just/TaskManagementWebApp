using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskManagementWebApp.Models;

namespace TaskManagementWebApp.Controllers
{
    public class AppUsersController : Controller
    {
        private TMDBEntities db = new TMDBEntities();


        private AppUser CurrentUser()
        {
            int? userId = Session["UserId"] as int?;
            if (userId.HasValue)
            {
                return db.AppUser.Find(userId.Value);
            }
            return null;
        }

        private bool IsUserAdmin()
        {
            var user = CurrentUser();
            return user != null && user.IsAdmin;
        }

        // GET: AppUsers
        public ActionResult Index()
        {
            if (!IsUserAdmin())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(db.AppUser.ToList());
        }

        // GET: AppUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUser.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // GET: AppUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Username,Email,PasswordHash,DateCreated,LastLoginDate,IsAdmin")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                // Check if this is the first user
                if (!db.AppUser.Any())
                {
                    appUser.IsAdmin = true; // Make the first user an admin
                }
                // Hash the password before saving it
                appUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(appUser.PasswordHash);

                appUser.DateCreated = DateTime.Now;
                appUser.LastLoginDate = null;
                db.AppUser.Add(appUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appUser);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            var user = db.AppUser.FirstOrDefault(u => u.Username == username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Session["UserId"] = user.UserId; // Store user Id in session. Not using ASP.NET Identity for simplicity.
                Session["Username"] = user.Username;
                // Set the admin flag in session
                Session["IsAdmin"] = user.IsAdmin;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }

        public ActionResult Logout()
        {
            Session["UserId"] = null; // Clear session
            Session["Username"] = null;
            Session["IsAdmin"] = null;
            return RedirectToAction("Login");
        }




        // GET: AppUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUser.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Username,Email,PasswordHash,DateCreated,LastLoginDate,IsAdmin")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appUser);
        }

        // GET: AppUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser appUser = db.AppUser.Find(id);
            if (appUser == null)
            {
                return HttpNotFound();
            }
            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppUser appUser = db.AppUser.Find(id);
            db.AppUser.Remove(appUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
