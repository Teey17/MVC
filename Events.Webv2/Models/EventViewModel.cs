using Events.DataV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Events.Webv2.Models
{
    public class EventViewModel
    {
        public static Expression<Func<Event, EventViewModel>> ViewModel
        {
            get
            {
                return e => new EventViewModel()
                {
                    Id = e.Id,
                    Title = e.Title,
                    startDateTime = e.startDateTime,
                    Duration = e.Duration,
                    Author = e.Author.FullNme,
                    Location = e.Location
                };
            }
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime startDateTime { get; set; }

        public TimeSpan? Duration { get; set; }

        public string AuthorId { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }
        public string Location { get; set; }
    }
}