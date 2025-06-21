namespace whris.UI.Reports
{
    partial class RepAllowance
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.DataAccess.Sql.CustomSqlQuery customSqlQuery1 = new DevExpress.DataAccess.Sql.CustomSqlQuery();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter2 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter3 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter4 = new DevExpress.DataAccess.Sql.QueryParameter();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepAllowance));
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel27 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.pageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.ParamDateStart = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamDateEnd = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamBranchId = new DevExpress.XtraReports.Parameters.Parameter();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.ParamCompanyId = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel5,
            this.xrLabel6,
            this.xrLabel3,
            this.xrLabel1,
            this.xrLabel7,
            this.xrLabel27});
            this.Detail.HeightF = 23F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel5
            // 
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BiometricIdNumber]")});
            this.xrLabel5.Font = new DevExpress.Drawing.DXFont("Arial", 8F);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(202.0833F, 0F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(109.375F, 23F);
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UsePadding = false;
            this.xrLabel5.Text = "xrLabel7";
            // 
            // xrLabel6
            // 
            this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AllowanceAmount]")});
            this.xrLabel6.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(582.2917F, 0F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(67.70834F, 23F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UsePadding = false;
            this.xrLabel6.Text = "xrLabel23";
            this.xrLabel6.TextFormatString = "{0:N2}";
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DaysWithAllowance]")});
            this.xrLabel3.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(514.5833F, 0F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(67.70834F, 23F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UsePadding = false;
            this.xrLabel3.Text = "xrLabel23";
            this.xrLabel3.TextFormatString = "{0:N2}";
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NoOfDaysTardy1]")});
            this.xrLabel1.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(412.4998F, 0F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(102.0833F, 23F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UsePadding = false;
            this.xrLabel1.Text = "xrLabel23";
            this.xrLabel1.TextFormatString = "{0:N2}";
            // 
            // xrLabel7
            // 
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[FullName]")});
            this.xrLabel7.Font = new DevExpress.Drawing.DXFont("Arial", 8F);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel7.Multiline = true;
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(202.0833F, 23F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UsePadding = false;
            this.xrLabel7.Text = "xrLabel7";
            // 
            // xrLabel27
            // 
            this.xrLabel27.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NoOfDaysPresent1]")});
            this.xrLabel27.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel27.LocationFloat = new DevExpress.Utils.PointFloat(311.4583F, 0F);
            this.xrLabel27.Multiline = true;
            this.xrLabel27.Name = "xrLabel27";
            this.xrLabel27.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel27.SizeF = new System.Drawing.SizeF(101.0415F, 23F);
            this.xrLabel27.StylePriority.UseFont = false;
            this.xrLabel27.StylePriority.UsePadding = false;
            this.xrLabel27.Text = "xrLabel23";
            this.xrLabel27.TextFormatString = "{0:N2}";
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel12,
            this.xrLabel19,
            this.xrLabel10,
            this.xrLabel2,
            this.xrLabel4,
            this.xrLabel8,
            this.xrLabel9,
            this.xrLabel11,
            this.xrLabel13,
            this.xrLabel16,
            this.xrLabel18,
            this.xrLabel17});
            this.TopMargin.HeightF = 190F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel10
            // 
            this.xrLabel10.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel10.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(202.0834F, 131.5833F);
            this.xrLabel10.Multiline = true;
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(109.3751F, 48.41669F);
            this.xrLabel10.StylePriority.UseBorders = false;
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.Text = "Biometric";
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'From \' + FormatString(\'{0:MM-dd-yyyy}\', ?ParamDateStart) + \' to \' + FormatString" +
                    "(\'{0:MM-dd-yyyy}\', ?ParamDateEnd)")});
            this.xrLabel2.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 76.91662F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(391.6667F, 23F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Allowance Report";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel4
            // 
            this.xrLabel4.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel4.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(0.0001271566F, 131.5833F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(202.0833F, 48.41669F);
            this.xrLabel4.StylePriority.UseBorders = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.Text = "Employee";
            // 
            // xrLabel8
            // 
            this.xrLabel8.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel8.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(311.4584F, 131.5833F);
            this.xrLabel8.Multiline = true;
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(101.0415F, 48.41669F);
            this.xrLabel8.StylePriority.UseBorders = false;
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.Text = "Net # of Days(Present on Salary Period)";
            // 
            // xrLabel9
            // 
            this.xrLabel9.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel9.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(412.5F, 131.5833F);
            this.xrLabel9.Multiline = true;
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(102.0833F, 48.41669F);
            this.xrLabel9.StylePriority.UseBorders = false;
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.Text = "Total # of Days w/ Tardy";
            // 
            // xrLabel11
            // 
            this.xrLabel11.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel11.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(514.5834F, 131.5833F);
            this.xrLabel11.Multiline = true;
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(67.70831F, 48.41669F);
            this.xrLabel11.StylePriority.UseBorders = false;
            this.xrLabel11.StylePriority.UseFont = false;
            this.xrLabel11.Text = "Net # of Days w/ Allowance";
            // 
            // xrLabel13
            // 
            this.xrLabel13.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel13.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(582.2919F, 131.5833F);
            this.xrLabel13.Multiline = true;
            this.xrLabel13.Name = "xrLabel13";
            this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel13.SizeF = new System.Drawing.SizeF(67.70831F, 48.41669F);
            this.xrLabel13.StylePriority.UseBorders = false;
            this.xrLabel13.StylePriority.UseFont = false;
            this.xrLabel13.Text = "Allowance Amount";
            // 
            // xrLabel16
            // 
            this.xrLabel16.Font = new DevExpress.Drawing.DXFont("Arial", 12F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(0.0001271566F, 53.74999F);
            this.xrLabel16.Multiline = true;
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel16.SizeF = new System.Drawing.SizeF(650F, 23F);
            this.xrLabel16.StylePriority.UseFont = false;
            this.xrLabel16.StylePriority.UseTextAlignment = false;
            this.xrLabel16.Text = "Allowance Report";
            this.xrLabel16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel18
            // 
            this.xrLabel18.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(415.6248F, 99.74995F);
            this.xrLabel18.Multiline = true;
            this.xrLabel18.Name = "xrLabel18";
            this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel18.SizeF = new System.Drawing.SizeF(71.87499F, 23F);
            this.xrLabel18.StylePriority.UseFont = false;
            this.xrLabel18.Text = "Branch:";
            // 
            // xrLabel17
            // 
            this.xrLabel17.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Branch]")});
            this.xrLabel17.Font = new DevExpress.Drawing.DXFont("arial", 9F);
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(487.5F, 99.74997F);
            this.xrLabel17.Multiline = true;
            this.xrLabel17.Name = "xrLabel17";
            this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel17.SizeF = new System.Drawing.SizeF(162.5F, 23F);
            this.xrLabel17.StylePriority.UseFont = false;
            this.xrLabel17.Text = "xrLabel12";
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageInfo1,
            this.pageInfo2});
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // pageInfo1
            // 
            this.pageInfo1.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 77F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.pageInfo1.SizeF = new System.Drawing.SizeF(325F, 23F);
            this.pageInfo1.StylePriority.UseFont = false;
            // 
            // pageInfo2
            // 
            this.pageInfo2.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.pageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(325F, 77F);
            this.pageInfo2.Name = "pageInfo2";
            this.pageInfo2.SizeF = new System.Drawing.SizeF(325F, 23F);
            this.pageInfo2.StylePriority.UseFont = false;
            this.pageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.pageInfo2.TextFormatString = "Page {0} of {1}";
            // 
            // ParamDateStart
            // 
            this.ParamDateStart.Name = "ParamDateStart";
            this.ParamDateStart.Type = typeof(global::System.DateTime);
            this.ParamDateStart.ValueInfo = "1753-01-01";
            // 
            // ParamDateEnd
            // 
            this.ParamDateEnd.Name = "ParamDateEnd";
            this.ParamDateEnd.Type = typeof(global::System.DateTime);
            this.ParamDateEnd.ValueInfo = "1753-01-01";
            // 
            // ParamBranchId
            // 
            this.ParamBranchId.AllowNull = true;
            this.ParamBranchId.Name = "ParamBranchId";
            this.ParamBranchId.Type = typeof(int);
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "DefaultConnection";
            this.sqlDataSource1.Name = "sqlDataSource1";
            customSqlQuery1.Name = "Query";
            queryParameter1.Name = "ParamDateStart";
            queryParameter1.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("?ParamDateStart", typeof(System.DateTime));
            queryParameter2.Name = "ParamDateEnd";
            queryParameter2.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter2.Value = new DevExpress.DataAccess.Expression("?ParamDateEnd", typeof(System.DateTime));
            queryParameter3.Name = "ParamBranchId";
            queryParameter3.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter3.Value = new DevExpress.DataAccess.Expression("?ParamBranchId", typeof(int));
            queryParameter4.Name = "ParamCompanyId";
            queryParameter4.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter4.Value = new DevExpress.DataAccess.Expression("?ParamCompanyId", typeof(int));
            customSqlQuery1.Parameters.AddRange(new DevExpress.DataAccess.Sql.QueryParameter[] {
            queryParameter1,
            queryParameter2,
            queryParameter3,
            queryParameter4});
            customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            customSqlQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel14,
            this.xrLabel15,
            this.xrLine1});
            this.ReportFooter.HeightF = 33F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrLabel14
            // 
            this.xrLabel14.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([AllowanceAmount])")});
            this.xrLabel14.Font = new DevExpress.Drawing.DXFont("Arial", 7F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(582.2916F, 10F);
            this.xrLabel14.Multiline = true;
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(67.70834F, 23F);
            this.xrLabel14.StylePriority.UseFont = false;
            this.xrLabel14.StylePriority.UsePadding = false;
            xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.xrLabel14.Summary = xrSummary1;
            this.xrLabel14.Text = "xrLabel23";
            this.xrLabel14.TextFormatString = "{0:N2}";
            // 
            // xrLabel15
            // 
            this.xrLabel15.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel15.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10F);
            this.xrLabel15.Multiline = true;
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(582.2917F, 23F);
            this.xrLabel15.StylePriority.UseBorders = false;
            this.xrLabel15.StylePriority.UseFont = false;
            this.xrLabel15.Text = "Total";
            // 
            // xrLine1
            // 
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 4.083315F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(646.875F, 2F);
            // 
            // ParamCompanyId
            // 
            this.ParamCompanyId.Name = "ParamCompanyId";
            this.ParamCompanyId.Type = typeof(int);
            this.ParamCompanyId.ValueInfo = "0";
            // 
            // xrLabel12
            // 
            this.xrLabel12.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Company]")});
            this.xrLabel12.Font = new DevExpress.Drawing.DXFont("arial", 9F);
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(487.5F, 76.74998F);
            this.xrLabel12.Multiline = true;
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(162.5F, 23F);
            this.xrLabel12.StylePriority.UseFont = false;
            this.xrLabel12.Text = "xrLabel12";
            // 
            // xrLabel19
            // 
            this.xrLabel19.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(415.6248F, 76.91659F);
            this.xrLabel19.Multiline = true;
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(71.875F, 22.83337F);
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.Text = "Company:";
            // 
            // RepAllowance
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "Query";
            this.DataSource = this.sqlDataSource1;
            this.Margins = new DevExpress.Drawing.DXMargins(100F, 100F, 190F, 100F);
            this.ParameterPanelLayoutItems.AddRange(new DevExpress.XtraReports.Parameters.ParameterPanelLayoutItem[] {
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.ParamDateStart, DevExpress.XtraReports.Parameters.Orientation.Horizontal),
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.ParamDateEnd, DevExpress.XtraReports.Parameters.Orientation.Horizontal),
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.ParamBranchId, DevExpress.XtraReports.Parameters.Orientation.Horizontal)});
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.ParamDateStart,
            this.ParamDateEnd,
            this.ParamBranchId,
            this.ParamCompanyId});
            this.Version = "22.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.Parameters.Parameter ParamDateStart;
        private DevExpress.XtraReports.Parameters.Parameter ParamDateEnd;
        private DevExpress.XtraReports.Parameters.Parameter ParamBranchId;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel27;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel14;
        private DevExpress.XtraReports.UI.XRLabel xrLabel15;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel13;
        private DevExpress.XtraReports.UI.XRLabel xrLabel16;
        private DevExpress.XtraReports.UI.XRLabel xrLabel18;
        private DevExpress.XtraReports.UI.XRLabel xrLabel17;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        private DevExpress.XtraReports.Parameters.Parameter ParamCompanyId;
        private DevExpress.XtraReports.UI.XRLabel xrLabel12;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
    }
}
