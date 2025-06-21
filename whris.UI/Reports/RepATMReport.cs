using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace whris.UI.Reports
{
    public partial class RepATMReport : DevExpress.XtraReports.UI.XtraReport
    {
        public RepATMReport()
        {
            InitializeComponent();
        }

        private void ATMReportAmount_SummaryCalculated(object sender, TextFormatEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text)) 
            {
                //ATMNumberToEnglish.Text = $"PESOS: {NumberToWordsConverter.NumberToWords(double.Parse(e.Text)).ToUpper()} ({e.Text})";
            }
        }
    }
}
