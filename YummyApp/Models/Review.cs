using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YummyApp.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public float Rating { get; set; }
        public string Content { get; set; }
    }
}