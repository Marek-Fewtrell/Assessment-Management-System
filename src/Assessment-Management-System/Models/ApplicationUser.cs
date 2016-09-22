using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Assessment_Management_System.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Submission> AssessmentSubmissions { get; set; }
        public virtual ICollection<Assessment> Assessments { get; set; }
    }
}
