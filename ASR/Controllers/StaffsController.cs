﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ASR.Models;
using ASR.Data;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;

namespace ASR.Controllers
{
    [Authorize(Roles = Constants.StaffRole)]
    public class StaffsController : Controller
    {
 
        private const int ROOMSLOTMAX = 2;
        private const int STAFFSLOTMAX = 4;
        readonly string baseUrl;

        public StaffsController()
        {
            baseUrl = "https://localhost:44317/ASRapi/";
        }
 
        // GET: Show Staff homepage
        public async Task<IActionResult> Index(string id)
        {           
            if (id == null)
            {
                return NotFound();
            }

            // Show staff's appointments with students today
            ViewBag.Message = "";
            ViewBag.id = id;
            var staffId = id.Substring(0, 6);
            Staff staff = await GetStaff(staffId);
            ViewBag.StaffName = $"{staff.FirstName} {staff.LastName}";

            //Call All staff's slot from ASR web api
            List<Slot> staffSlots = new List<Slot>();
            using (var client = new HttpClient())
            {
                //Parsing service base url
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                //Sending request to web api
                HttpResponseMessage req = await client.GetAsync($"Staff/{staffId}/GetSlots");

                //Checking the response is successful or not
                if (req.IsSuccessStatusCode)
                {
                    //Storing the response detail received from web api
                    var slotResp = req.Content.ReadAsStringAsync().Result;

                    //Deserialize the response received from web api and storing to slot list
                    staffSlots = JsonConvert.DeserializeObject<List<Slot>>(slotResp);
                }
            }
            
            staffSlots = staffSlots
                .Where(s => (s.StartTime.Date == DateTime.Now.Date) && (s.StartTime.Hour > DateTime.Now.Hour) && (s.StudentID != null)).ToList();
            
            if (staffSlots == null || !staffSlots.Any())
            {
                ViewBag.Message = "You have no appointments today";
                return View();
            }
            
            return View(staffSlots);
        }

        // Generate different view
        public async Task<IActionResult> ListSchedules(string id, string searchDate, int? page)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Message = "";
            ViewBag.id = id;
            var staffId = id.Substring(0, 6);
            Staff staff = await GetStaff(staffId);
            ViewBag.StaffName = $"{staff.FirstName} {staff.LastName}";

            //Call All staff's slot from ASR web api
            List<Slot> staffSlots = new List<Slot>();
            using (var client = new HttpClient())
            {
                //Parsing service base url
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                //Sending request to web api
                HttpResponseMessage req = await client.GetAsync($"Staff/{staffId}/GetSlots");

                //Checking the response is successful or not
                if (req.IsSuccessStatusCode)
                {
                    //Storing the response detail received from web api
                    var slotResp = req.Content.ReadAsStringAsync().Result;

                    //Deserialize the response received from web api and storing to slot list
                    staffSlots = JsonConvert.DeserializeObject<List<Slot>>(slotResp);
                }
            }
            
            if (!String.IsNullOrEmpty(searchDate)) //If searching for a specific date
            {
                DateTime selectDate = Convert.ToDateTime(searchDate);
                if (selectDate.Date == DateTime.Now.Date)
                {
                    staffSlots = staffSlots.Where(s => (s.StartTime.Date == selectDate.Date) && (s.StartTime.Hour > DateTime.Now.Hour)).ToList();
                }
                else
                {
                    staffSlots = staffSlots.Where(s => (s.StartTime.Date == selectDate.Date)).ToList();
                }
            }
            else // If not looking for specific date, look for all future bookings
            {
                staffSlots = staffSlots.Where(s => s.StartTime.Date >= DateTime.Now.Date).ToList();
                List<Slot> slotToremove = new List<Slot>();
                foreach (var item in staffSlots)
                {
                    // Removing today's past bookings
                    if ((item.StartTime.Date == DateTime.Now.Date) && (item.StartTime.Hour < DateTime.Now.Hour))
                    {
                        slotToremove.Add(item);
                    }
                }
                staffSlots = staffSlots.Except(slotToremove).OrderBy(x => x.StartTime).ToList();
            }

            if (staffSlots == null || !staffSlots.Any())
            {
                ViewBag.Message = "No slots to show";
                return View();
            }

            int pageSize = 2;
            return View(await PaginatedList<Slot>.CreateAsync(staffSlots, page ?? 1, pageSize));
            
        }

        // GET: Staffs
        public async Task<IActionResult> ListSlots(string id, string searchDate, int? page)
        {         
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Message = "";
            ViewBag.id = id;
            var staffId = id.Substring(0, 6);
            Staff staff = await GetStaff(staffId);
            ViewBag.StaffName = $"{staff.FirstName} {staff.LastName}";
            
            //Call All staff's slot from ASR web api
            List<Slot> staffSlots = new List<Slot>();          
            using (var client = new HttpClient())
            {
                //Parsing service base url
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                //Sending request to web api
                HttpResponseMessage req = await client.GetAsync($"Staff/{staffId}/GetSlots");
                
                //Checking the response is successful or not
                if (req.IsSuccessStatusCode)
                {
                    //Storing the response detail received from web api
                    var slotResp = req.Content.ReadAsStringAsync().Result;

                    //Deserialize the response received from web api and storing to slot list
                    staffSlots = JsonConvert.DeserializeObject<List<Slot>>(slotResp);
                }
            }



            if (!String.IsNullOrEmpty(searchDate)) //If searching for a specific date
            {
                DateTime selectDate = Convert.ToDateTime(searchDate);
                if (selectDate.Date == DateTime.Now.Date)
                {
                    staffSlots = staffSlots.Where(s => (s.StartTime.Date == selectDate.Date) && (s.StartTime.Hour > DateTime.Now.Hour)).ToList();
                }
                else
                {
                    staffSlots = staffSlots.Where(s => (s.StartTime.Date == selectDate.Date)).ToList();
                }
            }
            else // If not looking for specific date, look for all future bookings
            {
                staffSlots = staffSlots.Where(s => s.StartTime.Date >= DateTime.Now.Date).ToList();
                List<Slot> slotToremove = new List<Slot>();
                foreach (var item in staffSlots)
                {
                    // Removing today's past bookings
                    if ((item.StartTime.Date == DateTime.Now.Date) && (item.StartTime.Hour < DateTime.Now.Hour))
                    {
                        slotToremove.Add(item);
                    }
                }
                staffSlots = staffSlots.Except(slotToremove).OrderBy(x => x.StartTime).ToList();
            }

            // Only show slots that are not yet booked by students
            List<Slot> bookedSlots = new List<Slot>();
            foreach (var item in staffSlots)
            {
                if(item.Student != null)
                {
                    bookedSlots.Add(item);
                }
                staffSlots = staffSlots.Except(bookedSlots).OrderBy(x => x.StartTime).ToList();
            }

            if (staffSlots == null || !staffSlots.Any())
            {
                ViewBag.Message = "No slots to delete";
                return View();
            }

            int pageSize = 2;
            return View(await PaginatedList<Slot>.CreateAsync(staffSlots, page ?? 1, pageSize));

            //return View(staffSlots);
        }
        
        public async Task<IActionResult> RoomAvailability(string id, string searchDate)
        {
            ViewBag.Message = "";
            if (id == null)
            {
                return NotFound();
            }
            
            ViewBag.Message = "";
            ViewBag.id = id;
            var staffId = id.Substring(0, 6);
            Staff staff = await GetStaff(staffId);

            List<RoomViewModel> roomAvail = new List<RoomViewModel>();

            List<Room> rooms = await GetAllRooms();
            List<Slot> slots = await GetAllSlots();

            DateTime selectDate = DateTime.Now; 

            if (!String.IsNullOrEmpty(searchDate)) //If searching for a specific date
            {
                selectDate = Convert.ToDateTime(searchDate);
            }

            ViewBag.dateToDisplay = selectDate.ToLongDateString();
            
            //Check every room in system if already created schedule on particular date
            foreach (Room rm in rooms)
            {
                var newRoom = new RoomViewModel { RoomName = rm.RoomName, Availability = ROOMSLOTMAX };
                foreach (Slot sl in slots)
                {
                    if (sl.RoomID == rm.RoomID && sl.StartTime.Date == selectDate.Date)
                    {
                        newRoom.Availability--;
                    }
                }
                roomAvail.Add(newRoom);
            }

            //If no room available or it is Sunday & Saturday
            if (roomAvail == null || !roomAvail.Any() || (selectDate.DayOfWeek == DayOfWeek.Saturday) || (selectDate.DayOfWeek == DayOfWeek.Sunday))
            {
                ViewBag.Message = "No rooms available.";
                return View();
            }
            return View(roomAvail);
        }
        

        // GET: Slots/Details/5
        public async Task<IActionResult> SlotDetails(string roomid,string startTime)
        {
            ViewBag.Message = "";
            Slot slot = await GetSlot(roomid, startTime);
            if (slot == null)
            {
                ViewBag.Message = "Not Schedule found.";
                return View();
            }
            ViewBag.id = slot.StaffID+"@rmit.edu.au";
            return View(slot);
        }

        // GET: Slots/Edit/5
        public async Task<IActionResult> SlotEdit(string roomid, string startTime)
        {
            ViewBag.Message = "";
            Slot slot = await GetSlot(roomid, startTime);
            ViewBag.id = slot.Staff.Email;

            if (slot == null)
            {
                ViewBag.Message = "Not Schedule found.";
                return View();
            }

            ViewData["StaffID"] = new SelectList(await GetAllStaffs(), "StaffID", "StaffID", slot.StaffID);
            ViewData["StudentID"] = new SelectList(await GetAllStudents(), "StudentID", "StudentID", slot.StudentID);
            return View(slot);
        }

        // POST: Slots/SlotEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SlotEdit(string roomid, string startTime, [Bind("RoomID,StartTime,StaffID,StudentID")] Slot slot)
        {
            ViewBag.Message = "";
            ViewBag.id = $"{slot.StaffID}@rmit.edu.au";
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    //Parsing service base url
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Clear();

                    //Serialize the slot object to json file
                    StringContent content = new StringContent(JsonConvert.SerializeObject(slot),Encoding.UTF8, "application/json");
                    //Sending request to web api
                    HttpResponseMessage reqSlot = await client.PutAsync($"Slot?roomid={roomid}&startTime={startTime}", content);
                    
                    //Checking whether the request is successfull or not
                    if(reqSlot.StatusCode == HttpStatusCode.NoContent || reqSlot.StatusCode == HttpStatusCode.OK)
                    {
                        ViewBag.Message= "Slot has been updated!";
                    }
                }
 
                return RedirectToAction(nameof(ListSlots),new { id = $"{slot.StaffID}@rmit.edu.au" });
            }
            ViewData["StaffID"] = new SelectList(await GetAllStaffs(), "StaffID", "StaffID", slot.StaffID);
            ViewData["StudentID"] = new SelectList(await GetAllStudents(), "StudentID", "StudentID", slot.StudentID);
            return View(slot);
        }

        public async Task<IActionResult> SlotCreateView(string id, string bookingDate)
        {
            if (!String.IsNullOrEmpty(bookingDate))
            {
                return RedirectToAction("SlotCreate", new { id, bookingDate });
            }
            return View();
        }


        // GET: Slots/Create
        public async Task<IActionResult> SlotCreate(string id, string bookingDate)
        {
            ViewBag.Message = "";
            ViewBag.ErrMessage = "";
            ViewBag.id = id;
            var staffId = id.Substring(0, 6);
            Staff currentStaff = await GetStaff(staffId);
            ViewBag.StaffName = currentStaff.FirstName + " " + currentStaff.LastName;
            DateTime selectDate = Convert.ToDateTime(bookingDate);

            List<Slot> allSlots = await GetAllSlots();

            //Create dropdown list for room
            List<Room> allRooms = await GetAllRooms();
            List<Room> roomToremove = new List<Room>();
            foreach (var room in allRooms)
            {
                // If there are already 2 slots in this room on this date, remove from selection
                var numslots = allSlots.Where(s => (s.RoomID == room.RoomID) && (s.StartTime.Date == selectDate.Date)).Count();
                if(numslots == 2)
                {
                    roomToremove.Add(room);
                }
            }
            allRooms = allRooms.Except(roomToremove).OrderBy(x => x.RoomName).ToList();

            List<SelectListItem> roomSelect = new SelectList(allRooms, "RoomID", "RoomName").ToList();
            ViewData["RoomID"] = roomSelect;


            // Limit the time slots allowed based on the staff prev bookings on the day
            List<Slot> staffDaySlot = await GetAllSlots();
            staffDaySlot = staffDaySlot.Where(s => (s.StaffID == staffId) && (s.StartTime.Date == selectDate.Date)).ToList();
            

            List<SelectListItem> timeSelect = new List<SelectListItem>();
            TimeSpan time = Room.OpeningTime;
            TimeSpan oneHour = new TimeSpan(1, 0, 0);

            //Create dropdown list for booking hours
            while (time < Room.ClosingTime)
            {
                if (staffDaySlot.FirstOrDefault(sds => (sds.StartTime.TimeOfDay == time)) == null) //If staff hasn't made booking at this time, add in as a selection
                {
                    timeSelect.Add(new SelectListItem { Value = time.ToString(), Text = time.ToString("hh\\:mm") });
                }
                time = time.Add(oneHour);
            }

            ViewData["StartHour"] = timeSelect;
            
            ViewData["StaffID"] = id.Substring(0, 6);
            ViewData["BookingDate"] = bookingDate;

            return View();
        }

        // POST: Slots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SlotCreate([Bind("RoomID,StartTime,StaffID,StudentID")] Slot slot, string StartHour)
        {
            ViewBag.Message = "";
            ViewBag.ErrMessage = "";
            ViewBag.id = $"{slot.StaffID}@rmit.edu.au";          
            slot.StudentID = null;
            slot.StartTime = slot.StartTime + TimeSpan.Parse(StartHour);

            if (ModelState.IsValid)
            {
                //Get all the slot with the same date with new slot created
                Staff currentStaff = await GetStaff(slot.StaffID);
                ViewBag.StaffName = currentStaff.FirstName + " " + currentStaff.LastName;

                List <Slot> allSlots = await GetAllSlots();

                // Check how many slots that staff has created in that date
                if (allSlots.Where(s => s.StartTime.Date == slot.StartTime.Date && s.StaffID == currentStaff.StaffID).Count() >= STAFFSLOTMAX)
                {
                    ViewBag.ErrMessage = "Unable to create slot. Maximum staff's slot is 4 slots per day, choose another day." ;
                    //return RedirectToAction(nameof(SlotCreate), new { id = $"{slot.StaffID}@rmit.edu.au" });
                    return View();
                }
                else
                {
                    // Check how many slots that room has been booked in that date
                    if(allSlots.Where(s => s.StartTime.Date == slot.StartTime.Date && s.RoomID == slot.RoomID).Count() >= ROOMSLOTMAX)
                    {
                        ViewBag.ErrMessage = "Unable to create slot. Maximum room's slot is 2 per day, choose another day or room.";
                        //return RedirectToAction(nameof(SlotCreate), new { id = $"{slot.StaffID}@rmit.edu.au" });
                        return View();
                    }
                    else
                    {
                        // Check if the staff has been created the same booking time
                        if(allSlots.Where(s=>s.RoomID == slot.RoomID && s.StartTime == slot.StartTime).Any())
                        {
                            ViewBag.ErrMessage ="Unable to create slot. Duplicate schedule, choose different time." ;
                            //return RedirectToAction(nameof(SlotCreate), new { id = $"{slot.StaffID}@rmit.edu.au" });
                            return View();
                        }
                        //Check if staff is already have the same slot time and date, but on the other room
                        var otherBooking = allSlots.Where(s => s.StartTime == slot.StartTime && s.StaffID == currentStaff.StaffID).FirstOrDefault();
                        if (otherBooking != null)
                        {
                            ViewBag.ErrMessage = $"You have already made a booking at this date and time in Room {otherBooking.Room.RoomName}";
                            return View();
                        }
                        else
                        {
                            // Valid schedule datetime, then execute to database slot
                            using (var client = new HttpClient())
                            {
                                //Parsing service base url
                                client.BaseAddress = new Uri(baseUrl);
                                client.DefaultRequestHeaders.Clear();

                                //Serialize the slot object to json file
                                StringContent content = new StringContent(JsonConvert.SerializeObject(slot), Encoding.UTF8, "application/json");
                                //Sending request to web api
                                HttpResponseMessage reqSlot = await client.PostAsync("Slot", content);

                                if (reqSlot.IsSuccessStatusCode)
                                {
                                    ViewBag.Message = "New Slot has been created";
                                    return View(new Slot());
                                }
                                else
                                {
                                    ViewBag.ErrMessage = "Unable to add to database. Error connection.";
                                    return View(slot);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ViewData["StaffID"] = Request.Form["StaffID"];
                List<SelectListItem> timeSelect = new List<SelectListItem>();
                TimeSpan time = Room.OpeningTime;
                TimeSpan oneHour = new TimeSpan(1, 0, 0);

                while (time < Room.ClosingTime)
                {
                    timeSelect.Add(new SelectListItem { Value = time.ToString(), Text = time.ToString("hh\\:mm") });
                    time = time.Add(oneHour);
                }
                ViewData["StartHour"] = timeSelect;
            }

            //Check if room is already selected
            if (string.IsNullOrEmpty(slot.RoomID))
            {
                ModelState.AddModelError("", "Please select a room");
            }

            ViewData["RoomID"] = new SelectList(await GetAllRooms(), "RoomID", "RoomName", slot.RoomID);

            return View(slot);
        }    

        // GET: Slot by roomid and startTime
        public async Task<IActionResult> SlotDelete(string roomid, string startTime)
        {
            ViewBag.Message = "";
            Slot slot = await GetSlot(roomid, startTime);
            if (slot == null)
            {
                ViewBag.Message = "Slot not found.";
                return View();
            }
          
            ViewBag.id = slot.Staff.Email;
            Staff currentStaff = await GetStaff(slot.Staff.Email);
            ViewBag.StaffName = currentStaff.FirstName + " " + currentStaff.LastName;
   
            return View(slot);
        }

        // POST: Slots delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SlotDelete([Bind("RoomID,StartTime,StaffID,StudentID")] Slot slot)
        {
            var staffId = slot.StaffID;
            ViewBag.id = staffId + "@rmit.edu.au";      
            var roomid = slot.RoomID;
            var startTime = slot.StartTime.ToString("dd/MM/yyyy HH:mm");

            using (var client = new HttpClient())
            {
                //Parsing service base url
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                HttpResponseMessage reqSlot = await client.DeleteAsync($"Slot?roomid={roomid}&startTime={startTime}");

                if (reqSlot.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Slot has been deleted";
                    return View();
                }
                else
                {
                    ViewBag.Message = "Server Error.";
                    return View();
                }
            }
        }

        //Get one staff
        private async Task<Staff> GetStaff(string staffId)
        {
            Staff staff = new Staff();
           
            using (var client = new HttpClient())
            {
                //Parsing service base url
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                //Sending request to web api
                HttpResponseMessage req = await client.GetAsync($"Staff/{staffId}");

                //Checking the response is successful or not
                if (req.IsSuccessStatusCode)
                {
                    //Storing the response detail received from web api
                    var StaffResp = req.Content.ReadAsStringAsync().Result;

                    //Deserialize the response received from web api and storing to slot list
                    staff = JsonConvert.DeserializeObject<Staff>(StaffResp);
                }
            }
            return staff;
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

        //Get All Rooms
        private async Task<List<Room>> GetAllRooms()
        {
            List<Room> allRooms = new List<Room>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();

                HttpResponseMessage reqRooms = await client.GetAsync($"Room/GetAllRooms");

                if (reqRooms.IsSuccessStatusCode)
                {
                    var roomResp = reqRooms.Content.ReadAsStringAsync().Result;

                    allRooms = JsonConvert.DeserializeObject<List<Room>>(roomResp);
                }
            }
            return allRooms;
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

    }
}
