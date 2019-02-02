using System;
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
        private StudentSlotViewModel SlotStudent;
        readonly string baseUrl;
        
        public StudentsController(ASRContext context)
        {
            baseUrl = "https://localhost:44317/ASRapi/";
            SlotStudent = new StudentSlotViewModel();
        }

        // Show Student Homepage
        public async Task<IActionResult> Index(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //ViewBag.id = id;
            //var studentId = id.Substring(0, 8);

            ////Get Student
            //Student student = await GetStudent(studentId);

            //if(student == null)
            //{
            //    return NotFound();
            //}
            //return View(student);


            // Index page show students' appointment today, if any
            ViewBag.id = id;
            ViewBag.Message = "";
            var studentId = id.Substring(0, 8);
            var student = await GetStudent(studentId);
            ViewBag.Student = student.FirstName + " " + student.LastName;
            List<Slot> slots = await GetAllSlots();

            if (student == null)
            {
                ViewBag.Message = "Student not found.";
                return View();
            }

            var studentSlots = slots.Where(s => s.StudentID == studentId);

            studentSlots = studentSlots.Where(s => (s.StartTime.Date == DateTime.Now.Date) && (s.StartTime.Hour > DateTime.Now.Hour)).ToList();

            if (studentSlots == null || !studentSlots.Any())
            {
                ViewBag.Message = "You have no appointments today";
                return View();
            }
            
            return View(studentSlots);
        }

        // GET: Students
        public async Task<IActionResult> ListSlots(string id, string searchDate, int? page)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.id = id;
            var studentId = id.Substring(0, 8);
            var student = await GetStudent(studentId);
            ViewBag.StudentName = student.FirstName + " " + student.LastName;
            ViewBag.Message = "";

            List<Slot> allSlots = new List<Slot>();
            allSlots = await GetAllSlots();

            if (!String.IsNullOrEmpty(searchDate)) //If searching for a specific date
            {
                DateTime selectDate = Convert.ToDateTime(searchDate);
                allSlots = allSlots.Where(s => s.StartTime.Date == selectDate.Date).ToList();
            }
            else // If not looking for specific date, look for future booking slots
            {
                allSlots = allSlots.Where(s => (s.StartTime.Date >= DateTime.Now.Date) && (s.StartTime.Hour > DateTime.Now.Hour)).ToList();
            }

            if (allSlots == null || !allSlots.Any())
            {
                ViewBag.Message = "No slot available.";
                return View();
            }
            
            int pageSize = 2;
            return View(await PaginatedList<Slot>.CreateAsync(allSlots, page ?? 1, pageSize));
        }

        // GET: Students/Details/5
        public async Task<IActionResult> SlotDetails(string id, string roomid,string startTime)
        {
            ViewBag.Message = "";
            ViewBag.id = id;
            var studentId = id.Substring(0, 8);
            var student = await GetStudent(studentId);
            ViewBag.StudentName = student.FirstName + " " + student.LastName;
            Slot slot = await GetSlot(roomid, startTime);
            if (slot == null)
            {
                ViewBag.Message = "No slot available.";
                return View();
            }

            return View(slot);
        }

        //Get Staff availability
        public async Task<IActionResult> StaffAvailability(string id , string searchStaff)
        {
            ViewBag.Message = "";
            ViewBag.id = id;
            var studentId = id.Substring(0, 8);
            var student = await GetStudent(studentId);
            ViewBag.StudentName = student.FirstName + " " + student.LastName;

            List<Slot> slots = await GetAllSlots();
            var staffId = slots.Select(s => s.StaffID).Distinct().OrderBy(s => s);
            
            var Slots = slots.Where(s => (s.StartTime.Date >= DateTime.Now.Date) && (s.StartTime.Hour > DateTime.Now.Hour));
            if (!string.IsNullOrEmpty(searchStaff))
                Slots = Slots.Where(s => ((s.StaffID == searchStaff)));

            if (Slots == null || !Slots.Any())
            {
                ViewBag.Message = "No future slot.";
            }

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

            return View(SlotStudent);
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeBooking(StudentSlotViewModel stdSlot)
        {
            ViewBag.Message = "";
            var dateSlot = stdSlot.slot.StartTime.Date;
            var timeSlot = Request.Form["timeSlot"];
            var studentId = stdSlot.student.StudentID;

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

            var allSlots = await GetAllSlots();

            //Check whether the student already make booking in the same day
            if (allSlots.Where(s => s.StudentID == bookedSlot.StudentID && s.StartTime.Date == bookedSlot.StartTime.Date).Any())
            {
                ViewBag.Message = "Student can only book 1 slot per day. Choose another day.";
                return View();
            }
            else
            {
                if (allSlots.Where(s => s.RoomID == bookedSlot.RoomID && s.StartTime == bookedSlot.StartTime && s.StudentID != null).Any())
                {
                    ViewBag.Message = "Slot schedule not exist or already booked. Choose another slot.";
                    return View();
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
                            return View();
                        }
                        else
                        {
                            ViewBag.Message = "Booking failed.";
                            return View(stdSlot);
                        }
                    }
                }
            }
        }

        public async Task<IActionResult> ListBookedSlots(string id, string searchDate, int? page)
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
                ViewBag.Message = "Student not found.";
                return View();
            }

            var studentSlots = slots.Where(s => (s.StudentID == studentId));

            if (!String.IsNullOrEmpty(searchDate))//If searching for a specific date
            {
                DateTime selectDate = Convert.ToDateTime(searchDate);
                studentSlots = studentSlots.Where(s => (s.StartTime.Date == selectDate.Date)).ToList();
            }
            else // If not looking for specific date, look for all future bookings
            {
                slots = slots.Where(s => (s.StartTime.Date >= DateTime.Now.Date) && (s.StartTime.Hour > DateTime.Now.Hour)).ToList();
            }

            if (studentSlots == null || !studentSlots.Any())
            {
                ViewBag.Message = "No Booking Schedule";
                return View();
            }

            int pageSize = 2;
            return View(await PaginatedList<Slot>.CreateAsync(studentSlots, page ?? 1, pageSize));

            //return View(studentSlots);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> CancelBooking(string id, string roomid, string startTime)
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
 
            if (student == null)
            {
                ViewBag.Message = "Student not found.";
                return View();
            }

            var studentSlot = await GetSlot(roomid, startTime);

            if (studentSlot == null)
            {
                ViewBag.Message = "No Booking Schedule";
                return View();
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
            ViewBag.Message = "";
            var studentId = Request.Form["StudentID"];
            var student = await GetStudent(studentId);
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
