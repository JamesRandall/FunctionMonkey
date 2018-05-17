using System;

namespace SimpleRestApi.Model
{
    public class BlogPost
    {
        public Guid PostId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }
    }
}
