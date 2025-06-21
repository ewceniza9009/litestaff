using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.MstSysTable.Commands;
using whris.Application.CQRS.SysTables.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;

namespace whris.UI.Pages.SysTables
{
    [Authorize]
    [Secure("MstTable")]
    public class IndexModel : PageModel
    {
        private IMediator _mediator;

        public SysTableDto Tables { get; set; } = new SysTableDto();

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            var sysTables = new GetSysTables();

            Tables = await _mediator.Send(sysTables);

            return await Task.Run(() => Page());
        }

        public async Task<IActionResult> OnPostSave(SysTableDto tables)
        {
            var saveSysTables = new SaveSysTable()
            {
                SysTable = tables
            };

            var result = await _mediator.Send(saveSysTables);

            return new JsonResult(new { Id = 0 });
        }
    }
}
