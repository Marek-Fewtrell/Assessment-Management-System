using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment_Management_System.Models
{
    public class Assessment
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Display(Name = "Due Date")]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt dddd dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        [Display(Name = "Teacher ID")]
        [ForeignKey("ApplicationUser")]
        public string teacherID { get; set; }

        public virtual ICollection<Submission> Submissions { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
