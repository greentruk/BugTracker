using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Ticket
    {
        public Ticket()
        {
            this.History = new HashSet<History>();
            this.Comments = new HashSet<Comment>();
            this.Attachments = new HashSet<Attachment>();
        }
        public int Id { get; set; }
        public string AuthorId { get; set; }
        public string AssigneeId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public int PriorityId { get; set; }
        public int ProjectId { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        public string Description { get; set; }


        public virtual ApplicationUser Author { get; set; }
        public virtual ApplicationUser Assignee { get; set; }
        public virtual Type Type { get; set; }
        public virtual Project Project { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<History> History { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        
    }
}