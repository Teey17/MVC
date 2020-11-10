using Events.DataV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Events.Webv2.Models
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }

        public static Expression<Func<Comment, CommentViewModel>> ViewModel
        {
            get
            {
                return e => new CommentViewModel()
                {
                    Text = e.Text,
                    Author = e.Author.FullNme,
                    Id = e.Id
                    
                };
            }
        }
    }
}