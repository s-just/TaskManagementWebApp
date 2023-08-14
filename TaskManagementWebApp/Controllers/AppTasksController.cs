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

        // GET: AppTasks
        public ActionResult Index()
        {
            if (!IsUserAdmin())
            {
                return RedirectToAction("Login", "AppUsers");
            }
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

        [HttpGet]
        public JsonResult CheckForNewTask()
        {
            bool hasUpcomingTask = false;
            string message = string.Empty;

            int? userId = Session["UserId"] as int?;
            if (!userId.HasValue)
            {
                return Json(new { HasNewTask = false, Message = "User not logged in" }, JsonRequestBehavior.AllowGet);
            }

            // Retrieve the list of already-notified tasks from the session. If it doesn't exist, initialize an empty list.
            List<int> notifiedTasks = Session["NotifiedTasks"] as List<int> ?? new List<int>();

            foreach (AppTask aT in db.AppTask)
            {
                if (aT.AssignedToUserId == userId.Value &&
                    aT.DueDate <= DateTime.Now.AddDays(1) &&
                    !notifiedTasks.Contains(aT.TaskId)) // Check if the task hasn't been notified yet
                {
                    hasUpcomingTask = true;

                    // Store the due date without hours and minutes
                    DateTime dueDate = aT.DueDate.Value.Date;

                    message = $"You have an upcoming task due within 24 hours.\n\nTask: {aT.Title} at {dueDate:dd/MM/yyyy}\nDescription:\n{aT.Description}";

                    // Add the task to the notified tasks list and save it in the session.
                    notifiedTasks.Add(aT.TaskId);
                    Session["NotifiedTasks"] = notifiedTasks;
                    break;
                }
            }

            return Json(new { HasNewTask = hasUpcomingTask, Message = message }, JsonRequestBehavior.AllowGet);
        }


        // Update status of a task
        [HttpPost]
        public JsonResult UpdateTaskStatus(int taskId, int status)
        {
            AppTask task = db.AppTask.Find(taskId);
            if (task == null)
            {
                return Json(new { Success = false, Message = "Task not found" });
            }

            task.Status = status;
            task.LastUpdated = DateTime.Now;
            db.SaveChanges();

            return Json(new { Success = true, Message = "Task status updated" });
        }
    }
}
