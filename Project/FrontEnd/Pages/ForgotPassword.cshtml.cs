using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontEnd.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Add your password reset logic here
            bool isEmailSent = await SendPasswordResetEmailAsync(Email);
            if (isEmailSent)
            {
                Message = "Password reset email has been sent.";
            }
            else
            {
                Message = "Failed to send password reset email.";
            }

            return Page();
        }

        private Task<bool> SendPasswordResetEmailAsync(string email)
        {
            return Task.FromResult(true);
        }
    }
}
