using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASR.Models
{
    public class Slot
    {
        [Column("RoomID")]
        [Required]
        [Display(Name = "Room Name")]
        public string RoomID { get; set; }
        
        public virtual Room Room { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [Column("StartTime")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }
        
        [Required]
        public string StaffID { get; set; }
        
        public virtual Staff Staff { get; set; }

        [Column("BookedInStudentID")]
        public string StudentID { get; set; } = null;
        
        public virtual Student Student { get; set; }
    }
}
