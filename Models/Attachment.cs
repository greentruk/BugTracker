using System;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Attachment
  
    {
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string FilePath { get; set; }

    [Required]
    public string Description { get; set; }

    public DateTimeOffset Created { get; set; }
    public string UserId { get; set; }
    public string AuthorID { get; set; }

    public virtual Ticket Ticket { get; set; }
    public virtual ApplicationUser User { get; set; }
}
}