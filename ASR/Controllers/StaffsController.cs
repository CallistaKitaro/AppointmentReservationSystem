using System;
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
        //private readonly ASRContext _context;
        private const int ROOMSLOTMAX = 2;
        readonly string baseUrl = "https://localhost:44317/ASRapi/";

        //public StaffsController(ASRContext context)
        //{
        //    _context = context;
        //}

        // GET: Show Staff homepage
        public async Task<IActionResult> Index(string id)
        {           
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.id = id;
            var staffId = id.Substring(0, 6);

            //Get staff
            Staff staff = await GetStaff(staffId);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staffs
        public async Task<IActionResult> ListSlots(string id)
        {         
            if (id == null)
            {
                return NotFound();
            }

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
            if (staffSlots == null)
            {
                return NotFound();
            }
            return View(staffSlots);
        }

        [HttpGet]
        public IActionResult RoomAvailability(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.id = id;

            return View();
        }

        //Get: Search Rooms availability
        [HttpPost]
        public async Task<IActionResult> ShowRoomAvailability(string id)
        {
            DateTime searchDate;
            if (Request.Form["SearchDate"] == "")
            {
                searchDate = DateTime.Today.Date;
            }
            else
            {
                searchDate = DateTime.Parse(Request.Form["SearchDate"]).Date;
            }
         
            ViewBag.SearchDate = searchDate.ToLongDateString();
            ViewBag.id = id;          
            var staffId = id.Substring(0, 6);

            List<RoomViewModel> roomAvail = new List<RoomViewModel>();

            List<Room> rooms = await GetAllRooms();
            List<Slot> slots = await GetAllSlots();

            //Check every room in system if already created schedule on particular date
            foreach (Room rm in rooms)
            {
                var newRoom = new RoomViewModel { RoomName = rm.RoomName, Availability = ROOMSLOTMAX };
                foreach (Slot sl in slots)
                {
                    if (sl.RoomID == rm.RoomID && sl.StartTime.Date == searchDate)
                    {
                        newRoom.Availability--;
                    }
                }
                roomAvail.Add(newRoom);
            }

            if (roomAvail == null)
            {
                return NotFound();
            }

            return View(roomAvail);
        }

        // GET: Slots/Details/5
        public async Task<IActionResult> SlotDetails(string roomid,string startTime)
        {
            Slot slot = await GetSlot(roomid, startTime);

            if (slot == null)
            {
                return NotFound();
            }
            ViewBag.id = slot.StaffID+"@rmit.edu.au";
            return View(slot);
        }

        // GET: Slots/Edit/5
        public async Task<IActionResult> SlotEdit(string roomid, string startTime)
        {          
            Slot slot = await GetSlot(roomid, startTime);
            ViewBag.id = slot.Staff.Email;

            if (slot == null)
            {
                return NotFound();
            }
            //ViewData["RoomID"] = new SelectList(await GetAllRooms(), "RoomID", "RoomName", slot.RoomID);
            ViewData["StaffID"] = new SelectList(await GetAllStaffs(), "StaffID", "StaffID", slot.StaffID);
            ViewData["StudentID"] = new SelectList(await GetAllStudents(), "StudentID", "StudentID", slot.StudentID);
            return View(slot);
        }

        // POST: Slots/SlotEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SlotEdit(string roomid, string startTime, [Bind("RoomID,StartTime,StaffID,StudentID")] Slot slot)
        {
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
            //ViewData["RoomID"] = new SelectList(await GetAllRooms(), "RoomID", "RoomName", slot.RoomID);
            ViewData["StaffID"] = new SelectList(await GetAllStaffs(), "StaffID", "StaffID", slot.StaffID);
            ViewData["StudentID"] = new SelectList(await GetAllStudents(), "StudentID", "StudentID", slot.StudentID);
            return View(slot);
        }

        // GET: Slots/Create
        public async Task<IActionResult> SlotCreate(string id)
        {
            ViewBag.id = id;
            List<SelectListItem> roomSelect = new SelectList(await GetAllRooms(), "RoomID", "RoomName").ToList();
            roomSelect.Insert(0, (new SelectListItem() { Text = "Select Room", Value = string.Empty }));
            ViewData["RoomID"] = roomSelect;

            //TODO: Limit the time slots allowed based on the room ?
            List<SelectListItem> timeSelect = new List<SelectListItem>();
            TimeSpan time = Room.OpeningTime;
            TimeSpan oneHour = new TimeSpan(1, 0, 0);

            while (time < Room.ClosingTime)
            {
                timeSelect.Add(new SelectListItem { Value = time.ToString(), Text = time.ToString("hh\\:mm") });
                time = time.Add(oneHour);
            }
            ViewData["StartHour"] = timeSelect;

            //TODO: Block time picker to only show future dates excluding Sat and Sun - not implemented

            //ViewData["StaffID"] = new SelectList(await GetAllStaffs(), "StaffID", "StaffID");
            ViewData["StaffID"] = id.Substring(0, 6);

            return View();
        }

        // POST: Slots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SlotCreate([Bind("RoomID,StartTime,StaffID,StudentID")] Slot slot, string StartHour)
        {
            ViewBag.id = $"{slot.StaffID}@rmit.edu.au";
            slot.StudentID = null;
            slot.StartTime = slot.StartTime + TimeSpan.Parse(StartHour);

            if (ModelState.IsValid)
            {
               using(var client = new HttpClient())
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
                        ViewBag.Message = "New Slot has been created"; // dummy 
                    }
                }

                return RedirectToAction(nameof(ListSlots),new { id = $"{slot.StaffID}@rmit.edu.au"});
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
            //ViewData["StaffID"] = new SelectList(await GetAllStaffs(), "StaffID", "StaffID", slot.StaffID);
            
            return View(slot);
        }    

        // GET: Slots/Delete/5
        public async Task<IActionResult> SlotDelete(string roomid, string startTime)
        {
            ViewBag.Message = "";
            Slot slot = await GetSlot(roomid, startTime);
            if (slot == null)
            {
                return NotFound();
            }
            else
            {
                if (slot.StudentID != null)
                {
                    //To Do : add logic so only the slot that already booked can be deleted
                }
            }

            ViewBag.id = slot.Staff.Email;
            //ViewBag.startTime = startTime;

            return View(slot);
        }

        // POST: Slots/Delete/5
        [HttpPost, ActionName("SlotDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SlotDelete([Bind("RoomID,StartTime,StaffID,StudentID")] Slot slot)
        {
            var staffId = slot.StaffID;
            ViewBag.id = staffId + "@rmit.edu.au";      
            var roomid = slot.RoomID;
            //var timeSlot = Request.Form["timeSlot"];
            //slot.StartTime = slot.StartTime + TimeSpan.Parse(timeSlot);
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
                }

            }

            return RedirectToAction(nameof(ListSlots),new { id = staffId+"@rmit.edu.au" });
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
