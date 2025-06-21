using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.MstUser.Commands;
using whris.Application.CQRS.MstUser.Queries;
using whris.Application.CQRS.MstUserForms.Queries;
using whris.Application.Dtos;
using whris.Data.Models;
using whris.UI.Authorization;
using whris.UI.Services.Datasources;

namespace whris.UI.Pages.MstUser
{
    [Authorize]
    [Secure("MstUser")]
    public class DetailModel : PageModel
    {
        private IMediator _mediator;

        public MstUserDetailDto UserDetail { get; set; } = new MstUserDetailDto();
        public MstUserComboboxDatasources ComboboxDataSources = MstUserComboboxDatasources.Instance;

        public DetailModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var user = new GetMstUserById()
            {
                Id = Id
            };

            UserDetail = await _mediator.Send(user);
        }

        public async Task OnGetAdd()
        {
            var addUser = new AddUser();

            UserDetail = await _mediator.Send(addUser);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteUser = new DeleteUser() 
            {
                Id = id
            };

            await _mediator.Send(deleteUser);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(MstUserDetailDto user)
        {
            var saveUser = new SaveUser()
            {
                User = user
            };

            var resultId = await _mediator.Send(saveUser);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostTurnPage(int id, string action)
        {
            var getUser = new GetMstUserIdByTurnPage()
            {
                Id = id,
                Action = action
            };

            var userId = await _mediator.Send(getUser);

            return new JsonResult(new { Id = userId });
        }

        public async Task<IActionResult> OnPostAddUserForm(int userId)
        {
            var getUserForm = new AddUserForm()
            {
                UserId = userId
            };

            return new JsonResult(await _mediator.Send(getUserForm));
        }

        public async Task<IActionResult> OnPostCopyUserForms(int userId) 
        {
            var copyUserForms = new GetMstUserFormsById()
            {
                Id = userId
            };

            return new JsonResult(await _mediator.Send(copyUserForms));
        }

        public async Task<IActionResult> OnGetForms()
        {
            var result = ComboboxDataSources.Forms;

            return new JsonResult(await Task.Run(() => result));
        }
    }
}
