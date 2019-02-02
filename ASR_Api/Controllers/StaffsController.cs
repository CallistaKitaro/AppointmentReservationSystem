using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASR_Api.Data;
using ASR_Api.Models;

namespace ASR_Api.Controllers
{
    [Route("ASRapi/Staff")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly ASRContext _context;

        public StaffsController(ASRContext context)
        {
            _context = context;
        }

        // GET: ASRapi/Staff/GetAllStaffs
        [HttpGet]
        [Route("GetAllStaffs")]
        public async Task<ActionResult<IEnumerable<Staff>>> GetAllStaffs()
        {
            var staffs = await _context.Staff.Include(s => s.StaffSlots).ToListAsync();

            return staffs;
        }

        // GET: ASRapi/Staff/staffId
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaff([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var staff = await _context.Staff.FindAsync(id);

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        // PUT: ASRapi/Staff/e12345
        [HttpPut("{id}")]
        public async Task<IActionResult> EditStaff([FromRoute] string id, [FromBody] Staff staff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != staff.StaffID)
            {
                return BadRequest();
            }

            _context.Entry(staff).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(id))
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

        // POST: ASRapi/Staff
        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] Staff staff)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Staff.Add(staff);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StaffExists(staff.StaffID))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStaff", new { id = staff.StaffID }, staff);
        }

        // DELETE: ASRapi/Staff/e12345
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();

            return Ok(staff);
        }

        //Get: ASRapi/Staff/e12345/GetSlots
        [HttpGet("{id}/GetSlots")]
        public async Task<IActionResult> GetSlots(string id)
        {
            var staff = await _context.Staff.Include(s => s.StaffSlots)
                .FirstOrDefaultAsync(s => s.StaffID == id);
            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff.StaffSlots);
        }

        private bool StaffExists(string id)
        {
            return _context.Staff.Any(e => e.StaffID == id);
        }
    }
}