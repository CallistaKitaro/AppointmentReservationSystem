using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASR.Models
{
    public class Admin
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression("^(e|E)\\d{5}$", ErrorMessage = "Invalid Admin ID")]
        public string AdminID { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"([a-zA-Z0-9_\-\.]+)\@rmit.edu.au", ErrorMessage = "Invalid Admin email")]
        public string Email { get; set; }

    }
}
