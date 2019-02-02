using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASR_Api.Data;
using ASR_Api.Models;
using System.Globalization;

namespace ASR_Api.Controllers
{
    [Route("ASRapi/Slot")]
    [ApiController]
    public class SlotsController : ControllerBase
    {
        private readonly ASRContext _context;

        public SlotsController(ASRContext context)
        {
            _context = context;
        }

        // GET: ASRapi/Slot
        [HttpGet]
        [Route("GetAllSlots")]
        public async Task<ActionResult<IEnumerable<Slot>>> GetAllSlots()
        {
            var slots = await _context.Slot.Include(s => s.Room).Include(s => s.Staff).Include(s => s.Student).ToListAsync();

            return slots;
        }

        // Get ; ASRapi/Slot/staffId
        [HttpGet]
        [Route("ByStaffId")]
        public async Task<ActionResult<IEnumerable<Slot>>> GetSlotByStaffId(string id)
        {
            var staffSlots = await _context.Slot.Where(s => s.StaffID == id).ToArrayAsync();

            return staffSlots;
        }

        // Get ; ASRapi/Slot/studentId
        [HttpGet]
        [Route("ByStudentId")]
        public async Task<ActionResult<IEnumerable<Slot>>> GetSlotByStudentId(string id)
        {
            var staffSlots = await _context.Slot.Where(s => s.StudentID == id).ToArrayAsync();

            return staffSlots;
        }

        //Get: ASRapi/Slot
        [HttpGet]
        public async Task<IActionResult> GetSlot(string roomid, string startTime)
        {
            var slotTime = DateTime.ParseExact(startTime, "dd/MM/yyyy HH:mm", null, DateTimeStyles.None);        
            var slot = await _context.Slot
                        .Include(s => s.Room)
                        .Include(s => s.Staff)
                        .Include(s => s.Student)
                        .FirstOrDefaultAsync(s => s.RoomID == roomid && s.StartTime == slotTime);

            if (slot == null)
            {
                return NotFound();
            }        
            return Ok(slot);
        }

        //Put : ASRapi/Slot
        [HttpPut]
        public async Task<IActionResult> EditSlot(string roomid, string startTime, [FromBody][Bind("RoomID,StartTime,StaffID,StudentID")] Slot slot)
        {
            var slotTime = DateTime.ParseExact(startTime, "dd/MM/yyyy HH:mm", null, DateTimeStyles.None);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (roomid != slot.RoomID && slotTime != slot.StartTime)
            {
                return BadRequest();
            }

            _context.Entry(slot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SlotExists(roomid,slotTime))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: ASRapi/Slot
        [HttpPost]
        public async Task<IActionResult> CreateSlot([FromBody][Bind("RoomID,StartTime,StaffID,StudentID")] Slot slot)
        {
            slot.StudentID = null;
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Slot.Add(slot);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SlotExists(slot.RoomID,slot.StartTime))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSlot", new { roomid = slot.RoomID, startTime = slot.StartTime.ToString() }, slot);
        }

        // DELETE: ASRapi/Slot/5
        [HttpDelete]
        public async Task<IActionResult> DeleteSlot(string roomid, string startTime)
        {
            var slotTime = DateTime.ParseExact(startTime, "dd/MM/yyyy HH:mm", null, DateTimeStyles.None);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var slot = await _context.Slot.FindAsync(roomid,slotTime);

            if (slot == null)
            {
                return NotFound();
            }

            _context.Slot.Remove(slot);
            await _context.SaveChangesAsync();

            return Ok(slot);
        }

        private bool SlotExists(string roomid, DateTime startTime)
        {
            return _context.Slot.Any(e => e.RoomID == roomid && e.StartTime == startTime);
        }
    }
}