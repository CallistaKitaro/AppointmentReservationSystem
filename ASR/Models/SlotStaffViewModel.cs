using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASR.Models
{
    public class SlotStaffViewModel
    {
        public List<Slot> Slots { get; set; }
        public SelectList staffID { get; set; }
        public string searchStaff { get; set; }
    }
}
