using ICSharpCode.SharpZipLib.Zip;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using whris.Application.Common;
using whris.Application.CQRS.MstEmployee.Queries;
using whris.Application.Dtos;
using whris.Application.Queries.Home;
using whris.Data.Data;
using whris.Data.Models;
using whris.UI.Services;
using static NuGet.Packaging.PackagingConstants;

namespace whris.UI.Pages
{
    public class IndexModel : PageModel
    {
        private IMediator _mediator;
        private IWebHostEnvironment _environment;
        public List<CalendarTask> CalendarEvents = new List<CalendarTask>();
        public FloatingRecords.FloatingRecord FloatingRecord = new FloatingRecords().Record;

        public IndexModel(IWebHostEnvironment environment, IMediator mediator)
        {
            _environment = environment;
            _mediator = mediator;
        }

        public void OnGet()
        {
            var calendarEvents = new List<CalendarTask>();

            using (var context = new HRISCalendarContext())
            {
                calendarEvents = context.CalendarTasks
                    .Where(x => x.Start.Date >= DateTime.Now.Date)
                    .Take(5)
                    .OrderBy(x => x.Start)
                    .ToList();

                CalendarEvents = calendarEvents;
            }

            if (HttpContext.Session.GetObject<SysCurrentDto>(SessionHelper.SessionKey) is null)
            {
                if (User.Claims.Count() > 0)
                {
                    var aspUserId = User.Claims.ToList()[0].Value;
                    var email = User.Claims.ToList()[1].Value;
                    var userId = Lookup.GetUserIdByAspUserId(aspUserId);

                    var sysCurrent = new SysCurrentDto 
                    { 
                        AspUserId= aspUserId,
                        Email = email,
                        CurrentUserId = userId, 
                        AdminSwitch = false
                    };

                    HttpContext.Session.SetObject(SessionHelper.SessionKey, sysCurrent);
                }               
            }

            External.WriteSettings();
        }

        public FileResult OnGetDownLoadZip()
        {
            var webRoot = _environment.WebRootPath;
            var fileName = "com.streetsmart.mhris.apk.zip";
            var tempOutput = webRoot + "/Mobile/" + fileName;

            using (ZipOutputStream IzipOutputStream = new ZipOutputStream(System.IO.File.Create(tempOutput)))
            {
                IzipOutputStream.SetLevel(9);
                byte[] buffer = new byte[4096];
                var fileList = new List<string>();

                fileList.Add(webRoot + "/Mobile/com.streetsmart.mhris.apk");

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