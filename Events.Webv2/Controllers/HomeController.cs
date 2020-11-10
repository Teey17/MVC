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
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            int numOfPages;

            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var comments = this.db.Comments.

                Where(e => isAdmin || (e.Author != null && e.AuthorId == currentUserId))
                .FirstOrDefault();

            var isAuthor = (comments != null && comments.Author != null && comments.Text != null && comments.AuthorId == currentUserId);
            this.ViewBag.CanEdit = isAuthor || isAdmin;

            IEnumerable<EventViewModel> events = this.db.Events
                .OrderBy(e => e.startDateTime)
                .Where(e => e.IsPublic)
                .Take(15)
                .Select(EventViewModel.ViewModel).ToList();

            int extraPage = events.Count() % 15;

            numOfPages = (this.db.Events.Count() / 15);

            if (extraPage != 0)
            {
                numOfPages += 1;
            }



            ViewBag.NumberOfPages = numOfPages;
            ViewBag.Page = 1;

            IEnumerable<EventViewModel> upcomingEvents = events.Where(e => e.startDateTime > DateTime.Now).ToList();

            IEnumerable<EventViewModel> passedEvents = events.Where(e => e.startDateTime <= DateTime.Now).ToList();

            return View(new UpcomingPassedEventViewModel()
            {
                UpcomingEvents = upcomingEvents,
                PassedEvents = passedEvents
            });

        }

        public ActionResult Paging(int pageNo)
        {
            int page = (pageNo - 1) * 15;
            int numOfPages;
            ViewBag.CanGoToNextPage = true;

            IEnumerable<EventViewModel> events = this.db.Events
               .OrderBy(e => e.startDateTime)
               .Where(e => e.IsPublic)
               .Skip(page)
               .Take(15)
               .Select(EventViewModel.ViewModel).ToList();

            if (events.Count() < 1)
            {
                ViewBag.CanGoToNextPage = false;
            }
            else
            {

                int extraPage = events.Count() % 15;

                numOfPages = (this.db.Events.Count() / 15);

                if (extraPage != 0 && events.Count() > 15)
                {
                    numOfPages += 1;
                }
                else
                {
                    if(events.Count() < 15)
                    {
                        ViewBag.CanGoToNextPage = false;
                    }
                    else
                    {
                        ViewBag.CanGoToNextPage = true;
                    }
                    
                }

                ViewBag.NumberOfPages = numOfPages;

                ViewBag.Page = pageNo;

                var upcomingEvents = events.Where(e => e.startDateTime > DateTime.Now);
                var passedEvents = events.Where(e => e.startDateTime <= DateTime.Now);

                return View("Index", new UpcomingPassedEventViewModel()
                {
                    UpcomingEvents = upcomingEvents,
                    PassedEvents = passedEvents
                });
            }

            return View("Index");
        }

        public ActionResult EventDetailsById(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var eventDetails = this.db.Events
                .Where(e => e.Id == id)
                .Where(e => e.IsPublic || isAdmin || (e.Author != null && e.AuthorId == currentUserId))
                .Select(EventDetailsViewModel.ViewModel)
                .FirstOrDefault();

            var isOwner = (eventDetails != null && eventDetails.AuthorId != null &&
                eventDetails.AuthorId == currentUserId);
            this.ViewBag.CanEdit = isOwner || isAdmin;

            return this.PartialView("_EventDetails", eventDetails);
        }

        [HttpGet]
        public ActionResult AddComment(int id)  
        {
            var comment = new CommentInputModel();
            comment.EventId = id;
            return View(comment);
        }

        [HttpGet]
        public ActionResult DeleteComment(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
           

            var comment = new CommentInputModel();

            var commentToDelete = this.db.Comments.Where(e => e.Id == id).SingleOrDefault();

            if (commentToDelete != null) 
            {
                var isOwner = (commentToDelete != null && commentToDelete.AuthorId != null &&
                                commentToDelete.AuthorId == currentUserId);  

                if(isAdmin || isOwner)
                {
                    comment.Text = commentToDelete.Text;
                    comment.EventId = commentToDelete.EventId;
                    comment.Id = commentToDelete.Id;

                    if (commentToDelete.Author != null)
                    {
                        comment.Author = commentToDelete.Author.FullNme;
                    }
                }
                else
                {
                    this.AddNotification("Cannot delete event #" + id, NotificationType.ERROR);
                    return this.RedirectToAction("My");
                }
            }
            return View(comment);
        }

        [HttpPost]
        public ActionResult DeleteComment(CommentInputModel viewModel)
        {
            var commentToDelete = this.db.Comments.Where(e => e.Id == viewModel.Id).SingleOrDefault();

            if(commentToDelete != null)
            {
                this.db.Comments.Remove(commentToDelete);
                this.AddNotification("Comment successfully deleted!", NotificationType.INFO);
                return this.RedirectToAction("My");
            }

            return View();
        }

        public ActionResult AddCommentById(CommentInputModel viewModel)
        {
            var comment = GetComment(viewModel);

            if(comment != null)
            {
                var eventToComment = LoadEvent(viewModel.EventId); 
                
                if(eventToComment != null)
                {
                    eventToComment.Comments.Add(comment);
                    this.db.SaveChanges();
                    this.AddNotification("Comment added!!", NotificationType.INFO);
                    return this.RedirectToAction("Index");
                }
            }
            return View("AddComment");
        }

        public Comment GetComment(CommentInputModel model)
        {
            var comment = new Comment();

            var currrentUserId = this.User.Identity.GetUserId();

            var auther = LoadUser(currrentUserId);

            comment.Text = model.Text;
            comment.EventId = model.EventId;

            if(auther != null)
            {
                comment.Author = auther;
            }

            return comment;
        }

        private Event LoadEvent(int id)
        {
            var eventToEdit = this.db.Events
                .Where(e => e.Id == id).FirstOrDefault();
            return eventToEdit;
        }

        private ApplicationUser LoadUser(string id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var user = this.db.Users
                .Where(e => e.Id == id).FirstOrDefault();
            return user;
        }

    }
}