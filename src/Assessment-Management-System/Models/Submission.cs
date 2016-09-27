using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment_Management_System.Models
{
    public class Submission
    {
        public int ID { get; set; }
        [Display(Name = "Student ID")]
        [ForeignKey("ApplicationUser")]
        public string studentID { get; set; }
        [Display(Name = "Assessment ID")]
        [ForeignKey("Assessment")]
        public int AssessmentID { get; set; }
        [Display(Name = "Submitted On")]
        [DisplayFormat(DataFormatString = "{0:hh:mm tt dddd dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime submittedOn { get; set; }
        [Display(Name = "File Name")]
        public string fileName { get; set; }
        public string storageFileName { get; set; }

        public virtual Assessment Assessment { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
