using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASR.Models
{
    public class StudentSlotViewModel
    {
        public Slot slot { get; set; } = new Slot();
        public Student student { get; set; } = new Student();
    }
}
