using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace whris.UI.Reports
{
    public partial class RepNoneATMReport : DevExpress.XtraReports.UI.XtraReport
    {
        public RepNoneATMReport()
        {
            InitializeComponent();
        }

        private void NoneATMReportAmount_SummaryCalculated(object sender, TextFormatEventArgs e)
        {
            
        }
    }
}
