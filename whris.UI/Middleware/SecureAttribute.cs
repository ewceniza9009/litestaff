using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using whris.Application.Library;

namespace whris.UI.Authorization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SecureAttribute : Attribute, IResourceFilter, IAsyncPageFilter
    {
        public string? FormName { get; set; }

        public SecureAttribute(string? formName)
        {
            FormName = formName;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var listPaths = new List<string>() 
            {
                "MstTable",
                "MstEmployeeLoan",
                "TrnOverTime",
                "TrnChangeShift",
                "RepDTR",
                "RepPayroll",
                "RepMandatory",
                "RepWithholdingTax",
                "RepLeave",
                "RepLoan",
                "RepAccounting",
                "RepBank",
                "RepDemographics",
            };            

            var httpContextUser = context.HttpContext.User;
            var httpContextRequest = context.HttpContext.Request;

            var httpContextRequestPathValue = httpContextRequest.Path.Value?.ToUpper();

            if (listPaths.Contains(FormName ?? "NA"))
            {
                httpContextRequestPathValue = $"/{FormName}".ToUpper();
            }

            var isIndex = httpContextRequestPathValue == $@"/{FormName}".ToUpper() || httpContextRequestPathValue == $@"/{FormName}/Index".ToUpper();
            var queryString = httpContextRequest.QueryString.Value ?? "None";

            var aspUserId = httpContextUser.Claims.ToList()[0].Value; 

            var formRights = Security.GetFormRights(FormName ?? "", aspUserId);

            if (isIndex && !(formRights?.CanView ?? false)) 
            {
                if (Security.IsAspUserIdLinked(aspUserId))
                {
                    context.Result = new RedirectToPageResult(@"../Messages/NoRights");
                }
                else 
                {
                    context.Result = new UnauthorizedResult();
                }
            }

            if (queryString.ToUpper().Contains("?ID=") && !(formRights?.CanEdit ?? false))
            {
                context.Result = new UnauthorizedResult();
            }

            if (queryString.ToUpper().Contains("?HANDLER=ADD") && !(formRights?.CanAdd ?? false))
            {
                context.Result = new UnauthorizedResult();
            }

            if (queryString.ToUpper().Contains("?HANDLER=SAVE") && !(formRights?.CanLock ?? false))
            {
                context.Result = new UnauthorizedResult();
            }

            if (queryString.ToUpper().Contains("?HANDLER=DELETE") && !(formRights?.CanDelete ?? false))
            {
                context.Result = new UnauthorizedResult();
            }

            if (queryString.ToUpper().Contains("?PARAMID") && !(formRights?.CanPrint ?? false))
            {
                context.Result = new UnauthorizedResult();
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            await next();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class Secure2Attribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.HttpContext.User.Claims.ToList().Count == 0) 
            {
                if(context.HttpContext.Request.Path.Value != "/DXXRDV") 
                {
                    context.Result = new UnauthorizedResult();
                }

                return;
            }

            var aspUserId = context.HttpContext.User.Claims.ToList()[0].Value;

            if (!Security.IsAspUserIdLinked(aspUserId))
            {
                context.Result = new UnauthorizedResult();
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            
        }
    }
}
