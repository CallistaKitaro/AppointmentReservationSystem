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
    [Route("ASRapi/Room")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly ASRContext _context;

        public RoomsController(ASRContext context)
        {
            _context = context;
        }

        // GET: ASRapi/Room/GetAllRooms
        [HttpGet]
        [Route("GetAllRooms")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAllRooms()
        {
            var rooms = await _context.Room.Include(r => r.Slots).ToListAsync();
            return rooms;
        }

        // GET: ASRapi/Room/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoom([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var room = await _context.Room.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        // PUT: ASRapi/Room/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditRoom([FromRoute] string id, [FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != room.RoomID)
            {
                return BadRequest();
            }

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
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

        // POST: ASRapi/Room
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Room.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoom", new { id = room.RoomID }, room);
        }

        // DELETE: ASRapi/Room/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Room.Remove(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        //Get: ASRapi/Room/1/GetSlots
        [HttpGet("{id}/GetSlots")]
        public async Task<IActionResult> GetSlots(string id)
        {
            var room = await _context.Room.Include(r => r.Slots)
                .FirstOrDefaultAsync(r => r.RoomID == id);
            if (room == null)
            {
                return NotFound();
            }

            return Ok(room.Slots);
        }

        private bool RoomExists(string id)
        {
            return _context.Room.Any(e => e.RoomID == id);
        }
    }
}