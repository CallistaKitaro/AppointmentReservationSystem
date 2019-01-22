using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASR.Models
{
    public class Room
    {
        public string RoomID { get; set; }

        [Required]
        [Display(Name = "Room Name")]
        [RegularExpression(@"([a-zA-Z])", ErrorMessage = "Room name can only be one letter string")]
        [StringLength(1)]
        public string RoomName { get; set; }

        public virtual ICollection<Slot> Slots { get; set; }

        public static TimeSpan OpeningTime { get; } = new TimeSpan(9, 0, 0);
        public static TimeSpan ClosingTime { get; } = new TimeSpan(15, 0, 0);
    }
}
