using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ProjectViewModel
    {
        public Project Project { get; set; }
        public MultiSelectList Users { get; set; }
        public MultiSelectList AbsentUsers { get; set; }
        public string[] SelectedCurrentUsers { get; set; }
        public string[] SelectedAbsentUsers { get; set; }

    }
}