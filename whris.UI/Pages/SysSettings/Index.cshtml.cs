using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.Dtos;
using whris.Data.Models;
using whris.UI.Authorization;

namespace whris.UI.Pages.SysSettings
{
    [Authorize]
    [Secure("SysSettings")]
    public class IndexModel : PageModel
    {
        public List<MstPeriod> Periods { get; set; } = new List<MstPeriod>();
        public void OnGet()
        {
            Periods = (List<MstPeriod>)(Common.GetPeriods()?.Value ?? new List<MstPeriod>());
        }
    }
}
