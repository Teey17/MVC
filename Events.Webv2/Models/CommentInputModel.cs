 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Events.Webv2.Models
{
    public class CommentInputModel
    {
        [Required(ErrorMessage ="No comment entered")]
        public string Text { get; set; }

        public int Id { get; set; }
        public string Author { get; set; }
        public int EventId { get; set; }
    }
}