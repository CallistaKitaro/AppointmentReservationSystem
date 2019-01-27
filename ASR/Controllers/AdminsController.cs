using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASR.Models;

namespace ASR.Controllers
{
    public class AdminsController : Controller
    {
        private readonly ASRContext _context;

        public AdminsController(ASRContext context)
        {
            _context = context;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            return View(await _context.Admin.ToListAsync());
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.AdminID == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminID,FirstName,LastName,Email")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AdminID,FirstName,LastName,Email")] Admin admin)
        {
            if (id != admin.AdminID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminID))
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
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.AdminID == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var admin = await _context.Admin.FindAsync(id);
            _context.Admin.Remove(admin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(string id)
        {
            return _context.Admin.Any(e => e.AdminID == id);
        }


        // For staff function if needed

        // GET: Staffs/Details/5
        //public async Task<IActionResult> StaffDetails(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var staff = await _context.Staff
        //        .FirstOrDefaultAsync(m => m.StaffID == id);
        //    if (staff == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(staff);
        //}

        // GET: Staffs/Create
        //public IActionResult StaffCreate()
        //{
        //    return View();
        //}

        // POST: Staffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> StaffCreate([Bind("StaffID,FirstName,LastName,Email")] Staff staff)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (StaffExists(staff.StaffID))
        //        {
        //            ModelState.AddModelError("", "User already exist");
        //            return View(staff);
        //        }
        //        if (_context.Staff.Any(e => e.Email == staff.Email))
        //        {
        //            ModelState.AddModelError("", "Email has already exist");
        //            return View(staff);
        //        }
        //        else
        //        {
        //            staff.StaffID = staff.StaffID.ToLower();
        //            staff.Email = staff.Email.ToLower();
        //            _context.Add(staff);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(ListSlots));
        //        }
        //    }
        //    return View(staff);
        //}

        //// GET: Staffs/Edit/5
        //public async Task<IActionResult> StaffEdit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var staff = await _context.Staff.FindAsync(id);
        //    if (staff == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(staff);
        //}

        //// POST: Staffs/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> StaffEdit(string id, [Bind("StaffID,FirstName,LastName,Email")] Staff staff)
        //{
        //    if (id != staff.StaffID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (_context.Staff.Any(e => (e.Email == staff.Email) && (e.StaffID != staff.StaffID)))
        //            {
        //                ModelState.AddModelError("", "Email has already exist");
        //                return View(staff);
        //            }
        //            else
        //            {
        //                staff.Email = staff.Email.ToLower();
        //                _context.Update(staff);
        //                await _context.SaveChangesAsync();
        //            }
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!StaffExists(staff.StaffID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(ListSlots));
        //    }
        //    return View(staff);
        //}

        //// GET: Staffs/Delete/5
        //public async Task<IActionResult> StaffDelete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var staff = await _context.Staff
        //        .FirstOrDefaultAsync(m => m.StaffID == id);
        //    if (staff == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(staff);
        //}

        //// POST: Staffs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> StaffDeleteConfirmed(string id)
        //{
        //    var staff = await _context.Staff.FindAsync(id);
        //    _context.Staff.Remove(staff);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(ListSlots));
        //}

        //private bool StaffExists(string id)
        //{
        //    return _context.Staff.Any(e => e.StaffID == id);
        //}

    }
}
