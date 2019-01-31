using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ASR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using ASR.Data;

namespace ASR.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly ASRContext _context;

        private readonly SignInManager<AccountUser> _signInManager;
        private readonly UserManager<AccountUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<AccountUser> signInManager,
            UserManager<AccountUser> userManager,
            ILogger<ExternalLoginModel> logger,
            ASRContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [RegularExpression(@"^s\d{7}@student.rmit.edu.au|e\d{5}@rmit.edu.au$",
                ErrorMessage = "RMIT University email only")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "User ID")]
            [RegularExpression(@"^s\d{7}|e\d{5}", ErrorMessage = "Invalid user ID")]
            public string UserID { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                // If email is already in use for login
                if (_context.Users.Any(e => e.Email == Input.Email))
                {
                    ModelState.AddModelError("", "This email has already exist");
                    return Page();
                }

                //if email is new but user id already exist
                if (_context.Users.Any(e => e.StaffID == Input.UserID) || _context.Users.Any(e => e.StudentID == Input.UserID))
                {
                    ModelState.AddModelError("", "This user ID has already exist");
                    return Page();
                }

                Regex staffIDRegex = new Regex("^(e|E)\\d{5}$");
                Regex studentIDRegex = new Regex("^(s|S)\\d{7}$");
                Regex staffEmailRegex = new Regex(@"([a-zA-Z0-9_\-\.]+)\@rmit.edu.au");
                Regex studentEmailRegex = new Regex(@"([a-zA-Z0-9_\-\.]+)\@student.rmit.edu.au");

                if ((staffEmailRegex.IsMatch(Input.Email) && !staffIDRegex.IsMatch(Input.UserID)) ||
                    (studentEmailRegex.IsMatch(Input.Email) && !studentIDRegex.IsMatch(Input.UserID)))
                {
                    ModelState.AddModelError("", "Email and ID type does not match");
                    return Page();
                }

                if (Input.Email.Substring(0, 6) != Input.UserID)
                {
                    ModelState.AddModelError("", "Please use your own RMIT email only");
                    return Page();
                }

                var user = new AccountUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user);

                if (staffEmailRegex.IsMatch(Input.Email))
                {
                    await _userManager.AddToRoleAsync(user, Constants.StaffRole);

                    var staff = new Staff
                    {
                        StaffID = Input.UserID,
                        FirstName = Input.FirstName,
                        Email = Input.Email,
                    };
                    //Adding staff into staff table
                    _context.Add(staff);
                    await _context.SaveChangesAsync();

                    //Adding the staffID column at AspNetUser Table
                    _context.Users.FirstOrDefault(u => u.Email == Input.Email).StaffID = staff.StaffID;
                    await _context.SaveChangesAsync();

                }
                else if (studentEmailRegex.IsMatch(Input.Email))
                {
                    await _userManager.AddToRoleAsync(user, Constants.StudentRole);

                    var student = new Student
                    {
                        StudentID = Input.UserID,
                        FirstName = Input.FirstName,
                        Email = Input.Email,
                    };
                    //Adding student into student table
                    _context.Add(student);
                    await _context.SaveChangesAsync();

                    //Adding the studentID column at AspNetUser Table
                    _context.Users.FirstOrDefault(u => u.Email == Input.Email).StudentID = student.StudentID;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception();
                }
                

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
