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

        public class InputModel
        {
            [Required]
            [Display(Name = "User ID")]
            public string UserID { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
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
                // Ensure only valid id with valid email can register
                Regex staffIDRegex = new Regex("^(e|E)\\d{5}$");
                Regex studentIDRegex = new Regex("^(s|S)\\d{7}$");
                Regex staffEmailRegex = new Regex(@"([a-zA-Z0-9_\-\.]+)\@rmit.edu.au");
                Regex studentEmailRegex = new Regex(@"([a-zA-Z0-9_\-\.]+)\@student.rmit.edu.au");

                if (staffIDRegex.IsMatch(Input.UserID) || studentIDRegex.IsMatch(Input.UserID))
                {
                    if (staffIDRegex.IsMatch(Input.UserID) && !staffEmailRegex.IsMatch(Input.Email))
                    {
                        ModelState.AddModelError("", "Invalid staff email");
                        return Page();
                    }

                    if (studentIDRegex.IsMatch(Input.UserID) && !studentEmailRegex.IsMatch(Input.Email))
                    {
                        ModelState.AddModelError("", "Invalid student email");
                        return Page();
                    }
                    
                    var user = new AccountUser { UserName = Input.Email, Email = Input.Email };
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (staffEmailRegex.IsMatch(user.UserName))
                    {
                        await _userManager.AddToRoleAsync(user, Constants.StaffRole);

                        var staff = new Staff
                        {
                            StaffID = Input.UserID,
                            FirstName = Input.FirstName,
                            LastName = Input.LastName,
                            Email = Input.Email,
                        };
                        _context.Add(staff);
                        await _context.SaveChangesAsync();
                    }
                    else if (studentEmailRegex.IsMatch(user.UserName))
                    {
                        await _userManager.AddToRoleAsync(user, Constants.StudentRole);

                        var student = new Student
                        {
                            StudentID = Input.UserID,
                            FirstName = Input.FirstName,
                            LastName = Input.LastName,
                            Email = Input.Email,
                        };
                        _context.Add(student);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        throw new Exception();
                    }


                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //    var callbackUrl = Url.Page(
                        //        "/Account/ConfirmEmail",
                        //        pageHandler: null,
                        //        values: new { userId = user.Id, code = code },
                        //        protocol: Request.Scheme);

                        //    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        //        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Invalid user ID");
                    return Page();
                }
            }
            
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
