using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ASR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using ASR.Data;
using System.Linq;

namespace ASR.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly ASRContext _context;

        private readonly SignInManager<AccountUser> _signInManager;
        private readonly UserManager<AccountUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<AccountUser> userManager,
            SignInManager<AccountUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ASRContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        // Ensure only valid id with valid email can register
        Regex staffIDRegex = new Regex("^(e|E)\\d{5}$");
        Regex studentIDRegex = new Regex("^(s|S)\\d{7}$");
        Regex staffEmailRegex = new Regex(@"([a-zA-Z0-9_\-\.]+)\@rmit.edu.au");
        Regex studentEmailRegex = new Regex(@"([a-zA-Z0-9_\-\.]+)\@student.rmit.edu.au");

        public class InputModel
        {
            [Required]
            [RegularExpression(@"^(s|S)\d{7}|(e|E)\d{5}", ErrorMessage = "Invalid user ID")]
            [Display(Name = "User ID")]
            public string UserID { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [RegularExpression(@"^(s|S)\d{7}@student.rmit.edu.au|(e|E)\d{5}@rmit.edu.au$", 
                ErrorMessage = "Valid RMIT University email only")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                // If email is already in use for login
                if(_context.Users.Any(e => e.Email == Input.Email.ToLower()) )
                {
                    ModelState.AddModelError("", "This email has already exist");
                    return Page();
                }

                //if email is new but user id already exist
                if (_context.Users.Any(e => e.StaffID == Input.UserID.ToLower()) || _context.Users.Any(e => e.StudentID == Input.UserID.ToLower()))
                {
                    ModelState.AddModelError("", "This user ID has already exist");
                    return Page();
                }

                
                if ((staffEmailRegex.IsMatch(Input.Email) && !staffIDRegex.IsMatch(Input.UserID)) ||
                    (studentEmailRegex.IsMatch(Input.Email) && !studentIDRegex.IsMatch(Input.UserID)) )
                {
                    ModelState.AddModelError("", "Email and ID type does not match");
                    return Page();
                }

                if (((staffEmailRegex.IsMatch(Input.Email)) && (Input.Email.Substring(0, 6).ToLower() != Input.UserID.ToLower())) ||
                    ((studentEmailRegex.IsMatch(Input.Email)) && (Input.Email.Substring(0, 8).ToLower() != Input.UserID.ToLower())))
                {
                    ModelState.AddModelError("", "Please use your own RMIT credentials only");
                    return Page();
                }

                var user = new AccountUser { UserName = Input.Email.ToLower(), Email = Input.Email.ToLower() };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (staffEmailRegex.IsMatch(Input.Email))
                {
                    await _userManager.AddToRoleAsync(user, Constants.StaffRole);

                    var staff = new Staff
                    {
                        StaffID = Input.UserID.ToLower(),
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        Email = Input.Email.ToLower(),
                    };
                    //Adding staff into staff table
                    _context.Add(staff);
                    await _context.SaveChangesAsync();

                    //Adding the staffID column at AspNetUser Table
                    _context.Users.FirstOrDefault(u => u.Email == Input.Email.ToLower()).StaffID = staff.StaffID;
                    await _context.SaveChangesAsync();
                }
                else if (studentEmailRegex.IsMatch(Input.Email))
                {
                    await _userManager.AddToRoleAsync(user, Constants.StudentRole);

                    var student = new Student
                    {
                        StudentID = Input.UserID.ToLower(),
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        Email = Input.Email.ToLower(),
                    };
                    //Adding student into student table
                    _context.Add(student);
                    await _context.SaveChangesAsync();

                    //Adding the studentID column at AspNetUser Table
                    _context.Users.FirstOrDefault(u => u.Email == Input.Email.ToLower()).StudentID = student.StudentID;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception();
                }


                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    ViewData["Message"] = "You have been successfully registered into our system";
                    ViewData["role"] = staffEmailRegex.IsMatch(Input.Email) ? "Staffs" : studentEmailRegex.IsMatch(Input.Email) ? "Students" : "Home";
                    ViewData["userID"] = Input.Email;
                    return Page();
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
