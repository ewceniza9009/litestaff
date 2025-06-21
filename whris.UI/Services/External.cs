using Newtonsoft.Json;
using whris.Application.Dtos;

namespace whris.UI.Services
{
    public class External
    {
        static IServiceProvider? services = null;

        public static IServiceProvider? Services
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                services = value;
            }
        }

        public static HttpContext? Current
        {
            get
            {
                IHttpContextAccessor? httpContextAccessor = services?.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                return httpContextAccessor?.HttpContext;
            }
        }

        public static SysCurrentDto? CurrentUserSession =>  Current?.Session.GetObject<SysCurrentDto>(SessionHelper.SessionKey);

        public static void WriteSettings()
        {
            if ((Current?.User?.Claims?.Count() ?? 0) > 0) 
            {
                var email = Current?.User?.Claims?.ToList()[1].Value ?? "noemail@noemail.com";
                var settingsFile = JsonConvert.SerializeObject(CurrentUserSession);

                if (!string.IsNullOrEmpty(email.Trim()))
                {
                    File.WriteAllText($@"{Path.GetTempPath()}\{email}.json", settingsFile);
                }
            }           
        }
    }
}
