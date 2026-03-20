using Newtonsoft.Json;
using whris.Application.Dtos;

namespace whris.UI.Services
{
    public class External
    {
        private static IServiceProvider? services = null;
        private static readonly object _fileLock = new object();

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

        public static SysCurrentDto? CurrentUserSession => Current?.Session.GetObject<SysCurrentDto>(SessionHelper.SessionKey);

        public static void WriteSettings()
        {
            if ((Current?.User?.Claims?.Count() ?? 0) > 0)
            {
                var email = Current?.User?.Claims?.ToList()[1].Value ?? "noemail@noemail.com";

                if (!string.IsNullOrEmpty(email.Trim()))
                {
                    var settingsFile = JsonConvert.SerializeObject(CurrentUserSession);
                    var filePath = Path.Combine(Path.GetTempPath(), $"{email}.json");

                    lock (_fileLock)
                    {
                        File.WriteAllText(filePath, settingsFile);
                    }
                }
            }
        }
    }
}