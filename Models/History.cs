using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class History
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Updated { get; set; }

        public virtual Ticket Ticket { get; set; }

    }
}
