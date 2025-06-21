using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.UI.Services;

namespace whris.UI.Pages
{
    [Authorize]
    public class CalendarModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
