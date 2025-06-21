using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Library;
using whris.UI.Services;

namespace whris.UI.Pages.LogToPrintSlip
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string MobileCode { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost() 
        {
            if (!string.IsNullOrEmpty(MobileCode))
            {
                var employeeId = MobileUtils.GetEmployeeByMobileCode(MobileCode);

                if (employeeId > 0)
                {
                    var key = EncryptionHelper.Encrypt(MobileCode);

                    var redirectUrl = $"/PrintSlip?key={key}";
                    return Redirect(redirectUrl);
                }
            }

            ModelState.AddModelError("MobileCode", "Mobile Code is not valide.");
            return Page();
        }
    }
}
