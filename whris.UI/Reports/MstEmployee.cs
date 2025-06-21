using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using whris.Application.Common;

namespace whris.UI.Reports
{
    public partial class MstEmployee : DevExpress.XtraReports.UI.XtraReport
    {
        public MstEmployee()
        {
            InitializeComponent();
        }

        private void MstEmployee_BeforePrint(object sender, CancelEventArgs e)
        {
            xrPictureBox1.ImageUrl = Lookup.GetCompletePictureFilePathByEmployeeId(int.Parse(Parameters["ParamId"]?.Value.ToString() ?? ""));
        }
    }
}
