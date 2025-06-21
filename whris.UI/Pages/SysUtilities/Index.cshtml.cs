using ICSharpCode.SharpZipLib.Zip;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.CQRS.MstEmployee.Commands;
using whris.Application.Dtos;
using whris.UI.Authorization;
using whris.UI.Services;

namespace whris.UI.Pages.SysUtilities
{
    [Authorize]
    [Secure("SysUtilities")]
    public class IndexModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private IMediator _mediator;

        public IndexModel(IWebHostEnvironment environment, IMediator mediator)
        {
            _environment = environment;
            _mediator = mediator;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostDelete()
        {
            Application.Library.Utilities.DeleteAllTransactions();

            return new JsonResult(await Task.Run(() => 0));
        }

        public async Task<IActionResult> OnPostUploadEmployees() 
        {
            IFormFile? file = Request.Form.Files[0];
            var strId = Request?.Form["Id"][0]?.ToString();

            var tmpUploadEmployees = new List<TmpUploadEmployee>();

            if (file is not null && file.Length > 0)
            {
                var filePath = Path.Combine(_environment.WebRootPath, "Uploads", file.FileName);
                var extension = Path.GetExtension(filePath)?.ToLower();
                string[] validFileTypes = { ".xls", ".xlsx", ".csv", ".txt" };

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                if (validFileTypes.Contains(extension))
                {
                    tmpUploadEmployees = FileUtil.ProcessEmployeUploads(filePath, extension);
                }
                else
                {
                    return new BadRequestObjectResult("File format is incorrect");
                }
            }
            else
            {
                return new BadRequestObjectResult("No file was uploaded.");
            }

            var uploads = new UploadEmployees()
            {
                Employees = tmpUploadEmployees
            };

            _ = await _mediator.Send(uploads);

            return new JsonResult("Ok");
        }

        public FileResult OnGetDownLoadZip()
        {
            var webRoot = _environment.WebRootPath;
            var fileName = "TestFiles.zip";
            var tempOutput = webRoot + "/TestFiles/" + fileName;

            using (ZipOutputStream IzipOutputStream = new ZipOutputStream(System.IO.File.Create(tempOutput)))
            {
                IzipOutputStream.SetLevel(9);
                byte[] buffer = new byte[4096];
                var fileList = new List<string>();

                fileList.Add(webRoot + "/TestFiles/DTR.xlsx");
                fileList.Add(webRoot + "/TestFiles/Leave.xlsx");
                fileList.Add(webRoot + "/TestFiles/OtherDeduction.xlsx");
                fileList.Add(webRoot + "/TestFiles/OtherIncome.xlsx");
                fileList.Add(webRoot + "/TestFiles/Overtime.xlsx");
                fileList.Add(webRoot + "/TestFiles/Employees.xlsx");

                for (int i = 0; i < fileList.Count; i++)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(fileList[i]));
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    IzipOutputStream.PutNextEntry(entry);

                    using (FileStream oFileStream = System.IO.File.OpenRead(fileList[i]))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = oFileStream.Read(buffer, 0, buffer.Length);
                            IzipOutputStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                IzipOutputStream.Finish();
                IzipOutputStream.Flush();
                IzipOutputStream.Close();
            }

            byte[] finalResult = System.IO.File.ReadAllBytes(tempOutput);
            if (System.IO.File.Exists(tempOutput))
            {
                System.IO.File.Delete(tempOutput);
            }
            if (finalResult == null || !finalResult.Any())
            {
                throw new Exception(String.Format("Nothing found"));

            }

            return File(finalResult, "application/zip", fileName);
        }
    }
}
