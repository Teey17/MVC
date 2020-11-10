using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Events.Webv2.Models
{
    public class ImageHolder
    {
        public HttpPostedFileWrapper ImageFile { get; set; }
        public int EventId { get; set; }
    }
}