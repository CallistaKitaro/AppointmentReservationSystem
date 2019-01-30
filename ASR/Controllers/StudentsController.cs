﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASR.Models;
using Microsoft.AspNetCore.Authorization;
using ASR.Data;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Globalization;

namespace ASR.Controllers
{
    [Authorize(Roles = Constants.StudentRole)]
    public class StudentsController : Controller
    {
        private readonly ASRContext _context;
        readonly string baseUrl = "https://localhost:44317/ASRapi/";
        //public string Id { get; set; }
        private StudentSlotViewModel SlotStudent;

        public StudentsController(ASRContext context)
        {
            SlotStudent = new StudentSlotViewModel();
            _context = context;
        }

        // Show Student Homepage
        public async Task<IActionResult> Index(string id)
        {
            if (id == null)
            {
                id = SlotStudent.student.Email;
            }

            ViewBag.id = id;
            //Id = id;
            var studentId = id.Substring(0, 8);

            //Get Student
            Student student = await GetStudent(studentId);

            if(student == null)
            {
                return NotFound();
            }
            SlotStudent.student = student;

            //Id = student.Email;
            return View(SlotStudent);
        }

        // GET: Students
        public async Task<IActionResult> ListSlots(string id)
        {
            if (id == null)
            {
                id = SlotStudent.student.Email;
            }

            ViewBag.id = id;         
            List<Slot> allSlots = new List<Slot>();
            allSlots = await GetAllSlots();
            if(allSlots == null)
            {
                return NotFound();
            }

            return View(allSlots);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> SlotDetails(string roomid,string startTime)
        {
            Slot slot = await GetSlot(roomid, startTime);

            if (slot == null)
            {
                return NotFound();
            }
            SlotStudent.slot = slot;

            //ViewBag.id = Id;
            return View(SlotStudent);
        }

        //Get Staff availability
        public async Task<IActionResult> StaffAvailability(string id , string searchStaff)
        {
            ViewBag.id = id;
            List<Slot> slots = await GetAllSlots();
            var staffId = slots.Select(s => s.StaffID).Distinct().OrderBy(s => s);

            var Slots = slots.Select(s => s);
            if (!string.IsNullOrEmpty(searchStaff))
                Slots = slots.Where(s => s.StaffID == searchStaff);

            return View(new SlotStaffViewModel
            {
                staffID = new SelectList(staffId.ToList()),
                Slots = Slots.ToList()
            });
        }
        
        // GET: Students/Create
        [HttpGet]
        public async Task<IActionResult> MakeBooking(string id, string roomid, string startTime)
        {
            ViewBag.Message = "";

            Slot slot = await GetSlot(roomid, startTime);
            var studentId = id.Substring(0, 8);
            Student student = await GetStudent(studentId);
            SlotStudent.slot = slot;
            SlotStudent.student = student;
            ViewBag.id = id;
            ViewBag.Student = student.FirstName + " " + student.LastName;

            //var slots = await GetAllSlots();

            //if(student.StudentSlots.Count()>=1)
            //{
            //    if (slots.Where(s ))
            //    {
            //        ViewBag.Message = "You already ";
            //    }
                
            //}


            return View(SlotStudent);
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeBooking(StudentSlotViewModel stdSlot)
        {

            var dateSlot = stdSlot.slot.StartTime.Date;
            var timeSlot = Request.Form["timeSlot"];
            var studentId = Request.Form["StudentID"];
            Student student = await GetStudent(studentId);
            stdSlot.slot.StartTime = stdSlot.slot.StartTime + TimeSpan.Parse(timeSlot);
            var StartTime = stdSlot.slot.StartTime.ToString("dd/MM/yyyy HH:mm");
            ViewBag.id = studentId+"@student.rmit.edu.au";
            ViewBag.Student = student.FirstName + " " + student.LastName;

            Slot bookedSlot = new Slot
                { RoomID = stdSlot.slot.Room.RoomID,
                  StartTime = stdSlot.slot.StartTime,
                  StaffID = stdSlot.slot.StaffID,
                  StudentID = stdSlot.student.StudentID
                };
            
            if (bookedSlot.StudentID == "")
            {
                ModelState.AddModelError("", "StudentID still empty");
                return View(stdSlot);
            }

            var slots = await GetAllSlots();

            if (slots.Where(s => s.StartTime == bookedSlot.StartTime && s.StudentID == bookedSlot.StudentID).Any())
            {
                ModelState.AddModelError("", "Only can book one slot per day");
                return View(SlotStudent);
            }
            else
            {
               
                using (var client = new HttpClient())
                {
                    //Parsing service base url
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Clear();

                    //Serialize the slot object to json file
                    StringContent content = new StringContent(JsonConvert.SerializeObject(bookedSlot), Encoding.UTF8, "application/json");

                    //Sending request to web api
                    HttpResponseMessage req = await client.PutAsync($"Slot?roomid={bookedSlot.RoomID}&startTime={StartTime}", content);

                    //Checking whether the request is successfull or not
                    if (req.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Slot has been booked!";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Booking failled.");
                        return View(stdSlot);
                    }
                }
            }
            return RedirectToAction(nameof(ListSlots),new { id = $"{bookedSlot.StudentID}@student.rmit.edu.au"});
   
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> CancelBooking(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.id = id;
            ViewBag.Message = "";
            var studentId = id.Substring(0, 8);
            var student = await GetStudent(studentId);
            ViewBag.Student = student.FirstName + " " + student.LastName;
            List<Slot> slots = await GetAllSlots();

            if (student == null)
            {
                return NotFound();
            }

            var studentSlot = slots.FirstOrDefault(s => s.StudentID == studentId);

            if(studentSlot == null)
            {
                ViewBag.Message = "No Booking Schedule";
            }

            return View(studentSlot);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(Slot cancelledSlot)
        {
           
            var dateSlot = cancelledSlot.StartTime.Date;
            var timeSlot = Request.Form["timeSlot"];
            var studentId = Request.Form["StudentID"];
            var student = await GetStudent(studentId);
            cancelledSlot.StartTime = cancelledSlot.StartTime + TimeSpan.Parse(timeSlot);
            var StartTime = cancelledSlot.StartTime.ToString("dd/MM/yyyy HH:mm");
            ViewBag.id = studentId+"@student.rmit.edu.id";
            ViewBag.Student = student.FirstName + " " + student.LastName;

            Slot newSlot = new Slot
            {
                RoomID = cancelledSlot.Room.RoomID,
                StartTime = cancelledSlot.StartTime,
                StaffID = cancelledSlot.StaffID,
                StudentID = cancelledSlot.StudentID
            };

            var slots = await GetAllSlots();

            if (slots.Where(s => s.StudentID == studentId).Any())
            {
                newSlot.StudentID = null;

                using (var client = new HttpClient())
                {
                    //Parsing service base url
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Clear();

                    //Serialize the slot object to json file
                    StringContent content = new StringContent(JsonConvert.SerializeObject(newSlot), Encoding.UTF8, "application/json");

                    //Sending request to web api
                    HttpResponseMessage req = await client.PutAsync($"Slot?roomid={cancelledSlot.RoomID}&startTime={StartTime}", content);

                    //Checking whether the request is successfull or not
                    if (req.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Slot has been cancelled!";
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cancellation failed.");
                        cancelledSlot.StudentID = studentId;
                        return View(cancelledSlot);
                    }
                  
                }
            }
            else
            {
                ViewBag.Message = "Booking schedule not found.";
                return View();
            }
            //return RedirectToAction(nameof(ListSlots), new { id = $"{studentId}@student.rmit.edu.au"});

        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.StudentID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(string id)
        {
            return _context.Student.Any(e => e.StudentID == id);
        }

        //Get one student
        private async Task<Student> GetStudent(string studentId)
        {
            Student student = new Student();

            using (var client = new HttpClient())
            {
                //Parsing service base url
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                //Sending request to web api
                HttpResponseMessage req = await client.GetAsync($"Student/{studentId}");

                //Checking the response is successful or not
                if (req.IsSuccessStatusCode)
                {
                    //Storing the response detail received from web api
                    var StudentResp = req.Content.ReadAsStringAsync().Result;

                    //Deserialize the response received from web api and storing to slot list
                    student = JsonConvert.DeserializeObject<Student>(StudentResp);
                }
            }
            return student;
        }

        //Get One Slot 
        private async Task<Slot> GetSlot(string roomid, string startTime)
        {
            Slot getSlot = new Slot();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                HttpResponseMessage reqSlot = await client.GetAsync($"Slot?roomid={roomid}&startTime={startTime}");

                if (reqSlot.IsSuccessStatusCode)
                {
                    var slotResp = reqSlot.Content.ReadAsStringAsync().Result;

                    getSlot = JsonConvert.DeserializeObject<Slot>(slotResp);
                }
            }

            return getSlot;
        }

        //Get All Students
        private async Task<List<Student>> GetAllStudents()
        {
            List<Student> allStudents = new List<Student>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                HttpResponseMessage reqStudents = await client.GetAsync($"Student/GetAllStudents");

                if (reqStudents.IsSuccessStatusCode)
                {
                    var studResp = reqStudents.Content.ReadAsStringAsync().Result;

                    allStudents = JsonConvert.DeserializeObject<List<Student>>(studResp);
                }
            }
            return allStudents;
        }

        //Get All Staffs
        private async Task<List<Staff>> GetAllStaffs()
        {
            List<Staff> allStaffs = new List<Staff>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                HttpResponseMessage reqStaffs = await client.GetAsync($"Staff/GetAllStaffs");

                if (reqStaffs.IsSuccessStatusCode)
                {
                    var staffResp = reqStaffs.Content.ReadAsStringAsync().Result;

                    allStaffs = JsonConvert.DeserializeObject<List<Staff>>(staffResp);
                }
            }
            return allStaffs;
        }

        //Get All Slot
        private async Task<List<Slot>> GetAllSlots()
        {
            List<Slot> allSlots = new List<Slot>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                HttpResponseMessage reqSlots = await client.GetAsync($"Slot/GetAllSlots");

                if (reqSlots.IsSuccessStatusCode)
                {
                    var slotResp = reqSlots.Content.ReadAsStringAsync().Result;

                    allSlots = JsonConvert.DeserializeObject<List<Slot>>(slotResp);
                }
            }
            return allSlots;
        }



    }
}
