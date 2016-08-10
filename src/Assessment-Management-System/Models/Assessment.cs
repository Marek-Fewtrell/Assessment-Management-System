using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment_Management_System.Models
{
    public class Assessment
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [DisplayFormat(DataFormatString = "{0:hh:mm tt dddd dd MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
    }
}
