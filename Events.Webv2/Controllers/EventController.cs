using Events.DataV2;
using Events.Webv2.Extensions;
using Events.Webv2.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Events.Webv2.Controllers
{
    [Authorize]
    public class EventController : BaseController
    {
        // GET: Event
        [HttpGet]
        public ActionResult My()
        {
            string currentUserId = this.User.Identity.GetUserId();
            var events = this.db.Events
                .Where(e => e.AuthorId == currentUserId)
                .OrderBy(e => e.startDateTime)
                .Select(EventViewModel.ViewModel);

            var upcomingEvents = events.Where(e => e.startDateTime > DateTime.Now);
            var passedEvents = events.Where(e => e.startDateTime <= DateTime.Now);


            return View(new UpcomingPassedEventViewModel()
            {
                UpcomingEvents = upcomingEvents,
                PassedEvents = passedEvents
            });
        }
        public ActionResult Create()
        {
            ViewBag.EditCreate = "Create";
            return View();
        }

        public JsonResult Image(ImageHolder image)
        {
            var file = image.ImageFile;
            if(file != null)
            {
                file.SaveAs(Server.MapPath("/Images/event-" + image.EventId + ".png"));
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventInputModel model)
        {
            if(model != null && this.ModelState.IsValid)
            {
                var e = new Event()
                {
                    AuthorId = this.User.Identity.GetUserId(),
                    Title = model.Title,
                    startDateTime = model.startDateTime,
                    Description = model.Description,
                    Location = model.Location,
                    IsPublic = model.IsPublic
                };

                this.db.Events.Add(e);
                this.db.SaveChanges();

                this.AddNotification("Event Created.", NotificationType.INFO);

                return this.RedirectToAction("My");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var eventToEdit = this.LoadEvent(id);
            if(eventToEdit == null)
            {
                this.AddNotification("Cannot edit #" + id, NotificationType.ERROR);
                return this.RedirectToAction("My");
            }
            ViewBag.EditCreate = "Edit";
            var model = EventInputModel.CreateFromEvent(eventToEdit);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EventInputModel model)
        {
            var eventToEdit = this.LoadEvent(id);
            if (eventToEdit == null)
            {
                this.AddNotification("Cannot edit event #" + id, NotificationType.ERROR);
                return this.RedirectToAction("My");
            }

            if (model != null && this.ModelState.IsValid)
            {
                eventToEdit.Title = model.Title;
                eventToEdit.startDateTime = model.startDateTime;
                eventToEdit.Duration = model.Duration;
                eventToEdit.Description = model.Description;
                eventToEdit.Location = model.Location;
                eventToEdit.IsPublic = model.IsPublic;
                eventToEdit.Author.FullNme = model.Author;

                this.db.SaveChanges();
                this.AddNotification("Event edited", NotificationType.INFO);
                return this.RedirectToAction("My");
            }
            return this.View(model);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var eventToEdit = this.LoadEvent(id);

            if (eventToEdit == null)
            {
                this.AddNotification("Cannot edit #" + id, NotificationType.ERROR);
                return this.RedirectToAction("My");
            }
            ViewBag.EditCreate = "Delete";

            var model = EventInputModel.CreateFromEvent(eventToEdit);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, EventInputModel model)
        {
            var eventToDelete = this.LoadEvent(id);

            if (eventToDelete == null)
            {
                this.AddNotification("Cannot edit event #" + id, NotificationType.ERROR);
                return this.RedirectToAction("My");
            }

            if (model != null && this.ModelState.IsValid)
            {

                this.db.Events.Remove(eventToDelete);

                this.db.SaveChanges();
                this.AddNotification("Event deleted", NotificationType.INFO);
                return this.RedirectToAction("My");
            }
            return this.View(model);
        }

        private Event LoadEvent(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var eventToEdit = this.db.Events
                .Where(e => e.Id == id)
                .FirstOrDefault(e => e.AuthorId == currentUserId || isAdmin);
            return eventToEdit;
        }

        
    }
}