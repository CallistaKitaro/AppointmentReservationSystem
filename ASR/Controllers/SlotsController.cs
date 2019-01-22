using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASR.Models;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace ASR.Controllers
{
    public class SlotsController : Controller
    {
        private readonly ASRContext _context;

        public SlotsController(ASRContext context)
        {
            _context = context;
        }

        // GET: Slots
        public async Task<IActionResult> Index()
        {
            var aSRContext = _context.Slot.Include(s => s.Room).Include(s => s.Staff).Include(s => s.Student);
            return View(await aSRContext.ToListAsync());
        }

        // GET: Slots/Details/5
        public async Task<IActionResult> Details(string roomid, string startTime)
        {
            var slot = await _context.Slot
                .Include(s => s.Room)
                .Include(s => s.Staff)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => (m.RoomID == roomid) && (m.StartTime.ToString("MM/dd/yyyy HH:mm:ss") == startTime));
            if (slot == null)
            {
                return NotFound();
            }

            return View(slot);
            
        }

        // GET: Slots/Create
        public IActionResult Create()
        {
            List<SelectListItem> roomSelect = new SelectList(_context.Room, "RoomID", "RoomName").ToList();
            roomSelect.Insert(0, (new SelectListItem() { Text = "Select Room", Value = string.Empty }));
            ViewData["RoomID"] = roomSelect;
           
            //TODO: Limit the time slots allowed based on the room
            List<SelectListItem> timeSelect = new List<SelectListItem>();
            TimeSpan time = Room.OpeningTime;
            TimeSpan oneHour = new TimeSpan(1, 0, 0);

            while (time < Room.ClosingTime)
            {
                timeSelect.Add(new SelectListItem { Value = time.ToString(), Text = time.ToString("hh\\:mm") });
                time = time.Add(oneHour);
            }
            ViewData["StartHour"] = timeSelect;
            
            //TODO: Block time picker to only show future dates excluding Sat and Sun

            ViewData["StaffID"] = new SelectList(_context.Staff, "StaffID", "StaffID");

            return View();
        }

        // POST: Slots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomID,StartTime,StaffID,StudentID")] Slot slot, string StartHour)
        {
            slot.StudentID = null;
            slot.StartTime = slot.StartTime + TimeSpan.Parse(StartHour);

            if (ModelState.IsValid)
            {
                _context.Add(slot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //Check if room is already selected
            if (string.IsNullOrEmpty(slot.RoomID))
            {
                ModelState.AddModelError("", "Please select a room");
            }
            
            ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomName", slot.RoomID);
            ViewData["StaffID"] = new SelectList(_context.Staff, "StaffID", "StaffID", slot.StaffID);

            return View(slot);
        }

        // GET: Slots/Edit/5
        public async Task<IActionResult> Edit(string roomid, string startTime)
        {
            var slotTime = DateTime.ParseExact(startTime, "MM/dd/yyyy HH:mm:ss", null, DateTimeStyles.None);
            var slot = await _context.Slot.FindAsync(roomid, slotTime);
            if (slot == null)
            {
                return NotFound();
            }
            ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomName", slot.RoomID);
            ViewData["StaffID"] = new SelectList(_context.Staff, "StaffID", "StaffID", slot.StaffID);
            ViewData["StudentID"] = new SelectList(_context.Student, "StudentID", "StudentID", slot.StudentID);
            return View(slot);
        }

        // POST: Slots/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string roomid, string startTime, [Bind("RoomID,StartTime,StaffID,StudentID")] Slot slot)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(slot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SlotExists(slot.RoomID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomID"] = new SelectList(_context.Room, "RoomID", "RoomName", slot.RoomID);
            ViewData["StaffID"] = new SelectList(_context.Staff, "StaffID", "StaffID", slot.StaffID);
            ViewData["StudentID"] = new SelectList(_context.Student, "StudentID", "StudentID", slot.StudentID);
            return View(slot);
        }

        // GET: Slots/Delete/5
        public async Task<IActionResult> Delete(string roomid, string startTime)
        {
            var slot = await _context.Slot
                .Include(s => s.Room)
                .Include(s => s.Staff)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => (m.RoomID == roomid) && (m.StartTime.ToString("MM/dd/yyyy HH:mm:ss") == startTime));
            if (slot == null)
            {
                return NotFound();
            }

            return View(slot);
        }

        // POST: Slots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string roomid, string startTime)
        {
            var slotTime = DateTime.ParseExact(startTime, "MM/dd/yyyy HH:mm:ss", null, DateTimeStyles.None);
            var slot = await _context.Slot.FindAsync(roomid, slotTime);
            _context.Slot.Remove(slot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SlotExists(string id)
        {
            return _context.Slot.Any(e => e.RoomID == id);
        }
    }
}
