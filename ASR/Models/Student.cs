using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASR.Models
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Student ID")]
        [RegularExpression("^(s|S)\\d{7}$", ErrorMessage = "Invalid student ID")]
        public string StudentID { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"([a-zA-Z0-9_\-\.]+)\@student.rmit.edu.au", ErrorMessage = "Invalid student email")]
        public string Email { get; set; }
        
        public virtual ICollection<Slot> StudentSlots { get; set; }
    }
}
