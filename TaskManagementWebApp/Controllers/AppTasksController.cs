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
    public class AppTasksController : Controller
    {
        private TMDBEntities db = new TMDBEntities();

        // GET: AppTasks
        public ActionResult Index()
        {
            var appTask = db.AppTask.Include(a => a.AppUser).Include(a => a.AppUser1);
            return View(appTask.ToList());
        }

        public ActionResult MyTasks()
        {
            // Assuming you have a relationship set up between AppUser and AppTask
            int? userId = Session["UserId"] as int?;
            if (userId.HasValue)
            {
                var userTasks = db.AppTask.Where(t => t.AssignedToUserId == userId.Value).ToList();
                return View(userTasks);
            }
            else
            {
                return RedirectToAction("Login", "AppUsers");  // Or any other appropriate action
            }
        }

        // GET: AppTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppTask appTask = db.AppTask.Find(id);
            if (appTask == null)
            {
                return HttpNotFound();
            }
            return View(appTask);
        }

        // GET: AppTasks/Create
        public ActionResult Create()
        {
            ViewBag.AssignedToUserId = new SelectList(db.AppUser, "UserId", "Username");
            ViewBag.CreatedByUserId = new SelectList(db.AppUser, "UserId", "Username");
            return View();
        }

        // POST: AppTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TaskId,Title,Description,DueDate,Status,AssignedToUserId,CreatedByUserId,DateCreated,LastUpdated")] AppTask appTask)
        {
            if (ModelState.IsValid)
            {
                appTask.DateCreated = DateTime.Now; // Set DateCreated to the current date and time
                appTask.LastUpdated = null; // Initially, set LastUpdated date to null since the user hasn't updated it yet.
                db.AppTask.Add(appTask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignedToUserId = new SelectList(db.AppUser, "UserId", "Username", appTask.AssignedToUserId);
            ViewBag.CreatedByUserId = new SelectList(db.AppUser, "UserId", "Username", appTask.CreatedByUserId);
            return View(appTask);
        }

        // GET: AppTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppTask appTask = db.AppTask.Find(id);
            if (appTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedToUserId = new SelectList(db.AppUser, "UserId", "Username", appTask.AssignedToUserId);
            ViewBag.CreatedByUserId = new SelectList(db.AppUser, "UserId", "Username", appTask.CreatedByUserId);
            return View(appTask);
        }

        // POST: AppTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskId,Title,Description,DueDate,Status,AssignedToUserId,CreatedByUserId,DateCreated,LastUpdated")] AppTask appTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(db.AppUser, "UserId", "Username", appTask.AssignedToUserId);
            ViewBag.CreatedByUserId = new SelectList(db.AppUser, "UserId", "Username", appTask.CreatedByUserId);
            return View(appTask);
        }

        // GET: AppTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppTask appTask = db.AppTask.Find(id);
            if (appTask == null)
            {
                return HttpNotFound();
            }
            return View(appTask);
        }

        // POST: AppTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppTask appTask = db.AppTask.Find(id);
            db.AppTask.Remove(appTask);
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
