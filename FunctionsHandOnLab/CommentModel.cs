using System;

namespace FunctionsHandOnLab
{
    public class CommentModel
    {
        public string Text { get; set; }
        public string Status { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
