using whris.Application.Common;
using whris.Data.Models;

namespace whris.UI.Services.Datasources
{
    public class MstUserComboboxDatasources
    {
        public List<AspNetUser> AspNetUsers => (List<AspNetUser>)(Common.GetAspUsers()?.Value ?? new List<AspNetUser>());
        public List<MstUser> Users => (List<MstUser>)(Common.GetUsers()?.Value ?? new List<MstUser>());
        public List<SysForm> Forms => (List<SysForm>)(Common.GetForms()?.Value ?? new List<SysForm>());
        public static MstUserComboboxDatasources Instance => new MstUserComboboxDatasources();

        private MstUserComboboxDatasources()
        {

        }
    }
}
