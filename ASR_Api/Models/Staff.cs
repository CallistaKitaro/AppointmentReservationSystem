﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASR_Api.Models
{
    public class Staff
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Staff ID")]
        [RegularExpression("^(e|E)\\d{5}$", ErrorMessage = "Invalid staff ID")]
        public string StaffID { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"([a-zA-Z0-9_\-\.]+)\@rmit.edu.au", ErrorMessage = "Invalid staff email")]
        public string Email { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual ICollection<Slot> StaffSlots { get; set; } = new List<Slot>();
    }
}
