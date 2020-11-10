using Events.DataV2;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Events.Webv2.Models
{
    public class EventInputModel
    {
        private static EventInputModel eventInput;

        [Required(ErrorMessage ="Event title is required")]
        [StringLength(200,ErrorMessage ="The {0} must be between {1} and {2} characters long.", MinimumLength =1)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name ="Date and Time")]
        public DateTime startDateTime { get; set; }

        public TimeSpan? Duration { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }
        [MaxLength(200)]
        public string Location { get; set; }

        public bool IsPublic { get; set; }

        public static EventInputModel CreateFromEvent(Event eventObj)
        {
            eventInput = new EventInputModel();
            eventInput.Title = eventObj.Title;
            eventInput.startDateTime = eventObj.startDateTime;
            eventInput.Duration = eventObj.Duration;
            eventInput.Description = eventObj.Description;
            eventInput.Location = eventObj.Location;
            eventInput.IsPublic = eventObj.IsPublic;

            return eventInput;
        }
        
    }
}