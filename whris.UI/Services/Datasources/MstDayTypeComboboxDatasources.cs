using whris.Application.Common;
using whris.Data.Models;

namespace whris.UI.Services.Datasources
{
    public class MstDayTypeComboboxDatasources
    {
        public List<MstBranch> Branches => (List<MstBranch>)(Common.GetBranches()?.Value ?? new List<MstBranch>());

        public static MstDayTypeComboboxDatasources Instance => new MstDayTypeComboboxDatasources();

        private MstDayTypeComboboxDatasources()
        {

        }
    }
}
