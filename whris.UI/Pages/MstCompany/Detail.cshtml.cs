using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.MstCompany.Commands;
using whris.Application.CQRS.MstCompany.Queries;
using whris.Application.Dtos;
using whris.UI.Authorization;

namespace whris.UI.Pages.MstCompany
{
    [Authorize]
    [Secure("MstCompany")]
    public class DetailModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private IMediator _mediator;

        public MstCompanyDetailDto CompanyDetail { get; set; } = new MstCompanyDetailDto();

        public DetailModel(IWebHostEnvironment environment, IMediator mediator)
        {
            _environment = environment;
            _mediator = mediator;
        }

        public async Task OnGetAsync(int Id)
        {
            var company = new GetMstCompanyById()
            {
                Id = Id
            };

            CompanyDetail = await _mediator.Send(company);
        }

        public async Task OnGetAdd()
        {
            var addCompany = new AddCompany();

            CompanyDetail = await _mediator.Send(addCompany);
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var deleteCompany = new DeleteCompany()
            {
                Id = id
            };

            await _mediator.Send(deleteCompany);

            return new JsonResult(await Task.Run(() => id));
        }

        public async Task<IActionResult> OnPostSave(MstCompanyDetailDto company)
        {
            var saveCompany = new SaveCompany()
            {
                Company = company
            };

            var resultId = await _mediator.Send(saveCompany);

            return new JsonResult(resultId);
        }

        public async Task<IActionResult> OnPostTurnPage(int id, string action)
        {
            var getCompany = new GetMstCompanyIdByTurnPage()
            {
                Id = id,
                Action = action
            };

            var CompanyId = await _mediator.Send(getCompany);

            return new JsonResult(new { Id = CompanyId });
        }

        public async Task<IActionResult> OnPostAddBranch(int companyId)
        {
            var getBranch = new AddBranch()
            {
                CompanyId = companyId
            };

            return new JsonResult(await _mediator.Send(getBranch));
        }

        public async Task<IActionResult> OnPostUploadImage(IFormFile files)
        {
            var fileName = "";

            string photosFolder = Path.Combine(_environment.WebRootPath, "images");
            var companyId = int.Parse(Request?.Form["Id"][0] ?? "0");
            var oldPictureFilePath = Lookup.GetOldImageLogoPathByCompanyId(companyId);

            if (files != null)
            {
                fileName = files.FileName;
            }
            else
            {
                fileName = oldPictureFilePath;
            }

            if (oldPictureFilePath != fileName && oldPictureFilePath != null)
            {
                System.IO.File.Delete(Path.Combine(photosFolder, oldPictureFilePath));
            }


            if (files != null)
            {
                string itemImagePath = Path.Combine(photosFolder, files.FileName);

                using (var fileStream = new FileStream(itemImagePath, FileMode.Create))
                {
                    files.CopyTo(fileStream);
                }
                return new ObjectResult(new { status = "success" });
            }

            await Task.Run(() => { });

            return new JsonResult("Ok");
        }
    }
}
