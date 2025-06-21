namespace whris.UI.Reports
{
    partial class TrnOtherIncome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrnOtherIncome));
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary2 = new DevExpress.XtraReports.UI.XRSummary();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel39 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.pageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.ParamId = new DevExpress.XtraReports.Parameters.Parameter();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.EmployeeHeader = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.EmployeeFooter = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.xrLabel46 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLine3 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel23 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel3,
            this.xrLabel5,
            this.xrLabel1,
            this.xrLabel6,
            this.xrLabel8,
            this.xrLabel10,
            this.xrLabel12,
            this.xrLabel13,
            this.xrLabel2,
            this.xrLabel39,
            this.xrLabel18,
            this.xrLabel15});
            this.TopMargin.HeightF = 193.125F;
            this.TopMargin.Name = "TopMargin";
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PayrollGroup]")});
            this.xrLabel3.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(84.37503F, 137.2083F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(183.3333F, 23F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "xrLabel3";
            // 
            // xrLabel5
            // 
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[POINumber]")});
            this.xrLabel5.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(84.37503F, 68.20831F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(183.3333F, 23F);
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.Text = "xrLabel5";
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[POIRemarks]")});
            this.xrLabel1.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(84.37503F, 114.2083F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(565.625F, 23F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.Text = "xrLabel1";
            // 
            // xrLabel6
            // 
            this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[POIDate]")});
            this.xrLabel6.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(84.37503F, 91.2083F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.Text = "xrLabel6";
            this.xrLabel6.TextFormatString = "{0:d}";
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 68.20831F);
            this.xrLabel8.Multiline = true;
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(84.375F, 23F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.Text = "Name:";
            // 
            // xrLabel10
            // 
            this.xrLabel10.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 137.2083F);
            this.xrLabel10.Multiline = true;
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(84.375F, 23F);
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.Text = "Payroll Group:";
            // 
            // xrLabel12
            // 
            this.xrLabel12.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 114.2083F);
            this.xrLabel12.Multiline = true;
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(84.37499F, 23F);
            this.xrLabel12.StylePriority.UseFont = false;
            this.xrLabel12.Text = "Remarks:";
            // 
            // xrLabel13
            // 
            this.xrLabel13.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 91.2083F);
            this.xrLabel13.Multiline = true;
            this.xrLabel13.Name = "xrLabel13";
            this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel13.SizeF = new System.Drawing.SizeF(84.37499F, 23F);
            this.xrLabel13.StylePriority.UseFont = false;
            this.xrLabel13.Text = "Date Start:";
            // 
            // xrLabel2
            // 
            this.xrLabel2.Font = new DevExpress.Drawing.DXFont("Arial", 12F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(470.8333F, 68.20831F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(179.1666F, 23F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Payroll Other Inome";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrLabel39
            // 
            this.xrLabel39.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel39.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel39.LocationFloat = new DevExpress.Utils.PointFloat(546.875F, 169.6631F);
            this.xrLabel39.Multiline = true;
            this.xrLabel39.Name = "xrLabel39";
            this.xrLabel39.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel39.SizeF = new System.Drawing.SizeF(103.1249F, 23.46191F);
            this.xrLabel39.StylePriority.UseBorders = false;
            this.xrLabel39.StylePriority.UseFont = false;
            this.xrLabel39.StylePriority.UsePadding = false;
            this.xrLabel39.StylePriority.UseTextAlignment = false;
            this.xrLabel39.Text = "Amount";
            this.xrLabel39.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel18
            // 
            this.xrLabel18.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel18.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(253.1251F, 169.6631F);
            this.xrLabel18.Multiline = true;
            this.xrLabel18.Name = "xrLabel18";
            this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel18.SizeF = new System.Drawing.SizeF(293.7499F, 23.46191F);
            this.xrLabel18.StylePriority.UseBorders = false;
            this.xrLabel18.StylePriority.UseFont = false;
            this.xrLabel18.StylePriority.UsePadding = false;
            this.xrLabel18.StylePriority.UseTextAlignment = false;
            this.xrLabel18.Text = "Income";
            this.xrLabel18.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel15
            // 
            this.xrLabel15.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel15.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 169.6631F);
            this.xrLabel15.Multiline = true;
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(253.1251F, 23.46191F);
            this.xrLabel15.StylePriority.UseBorders = false;
            this.xrLabel15.StylePriority.UseFont = false;
            this.xrLabel15.StylePriority.UsePadding = false;
            this.xrLabel15.StylePriority.UseTextAlignment = false;
            this.xrLabel15.Text = "Name";
            this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageInfo2,
            this.pageInfo1});
            this.BottomMargin.Name = "BottomMargin";
            // 
            // pageInfo2
            // 
            this.pageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(325F, 77F);
            this.pageInfo2.Name = "pageInfo2";
            this.pageInfo2.SizeF = new System.Drawing.SizeF(325F, 23F);
            this.pageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.pageInfo2.TextFormatString = "Page {0} of {1}";
            // 
            // pageInfo1
            // 
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 77F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.pageInfo1.SizeF = new System.Drawing.SizeF(325F, 23F);
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel14,
            this.xrLabel7,
            this.xrLabel4});
            this.Detail.HeightF = 23F;
            this.Detail.Name = "Detail";
            // 
            // xrLabel14
            // 
            this.xrLabel14.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Amount]")});
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(546.875F, 0F);
            this.xrLabel14.Multiline = true;
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(103.1249F, 23F);
            this.xrLabel14.Text = "xrLabel14";
            this.xrLabel14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel14.TextFormatString = "{0:N2}";
            // 
            // xrLabel7
            // 
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OtherIncome]")});
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(253.1251F, 0F);
            this.xrLabel7.Multiline = true;
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(293.7499F, 23F);
            this.xrLabel7.Text = "xrLabel4";
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[FullName]")});
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 0F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(253.1251F, 23F);
            this.xrLabel4.Text = "xrLabel4";
            // 
            // ParamId
            // 
            this.ParamId.Name = "ParamId";
            this.ParamId.Type = typeof(int);
            this.ParamId.ValueInfo = "0";
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "DefaultConnection";
            this.sqlDataSource1.Name = "sqlDataSource1";
            customSqlQuery1.Name = "Query";
            queryParameter1.Name = "ParamId";
            queryParameter1.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("?ParamId", typeof(int));
            customSqlQuery1.Parameters.AddRange(new DevExpress.DataAccess.Sql.QueryParameter[] {
            queryParameter1});
            customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            customSqlQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // EmployeeHeader
            // 
            this.EmployeeHeader.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("FullName", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.EmployeeHeader.HeightF = 0F;
            this.EmployeeHeader.Name = "EmployeeHeader";
            // 
            // EmployeeFooter
            // 
            this.EmployeeFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel46,
            this.xrLabel9});
            this.EmployeeFooter.HeightF = 23.46191F;
            this.EmployeeFooter.Name = "EmployeeFooter";
            // 
            // xrLabel46
            // 
            this.xrLabel46.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel46.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel46.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel46.Multiline = true;
            this.xrLabel46.Name = "xrLabel46";
            this.xrLabel46.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel46.SizeF = new System.Drawing.SizeF(546.875F, 23.46191F);
            this.xrLabel46.StylePriority.UseBorders = false;
            this.xrLabel46.StylePriority.UseFont = false;
            this.xrLabel46.StylePriority.UsePadding = false;
            this.xrLabel46.StylePriority.UseTextAlignment = false;
            this.xrLabel46.Text = "Sub Total";
            this.xrLabel46.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel9
            // 
            this.xrLabel9.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([Amount])")});
            this.xrLabel9.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(546.875F, 0F);
            this.xrLabel9.Multiline = true;
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(103.1249F, 23F);
            this.xrLabel9.StylePriority.UseBorders = false;
            this.xrLabel9.StylePriority.UseFont = false;
            xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.xrLabel9.Summary = xrSummary1;
            this.xrLabel9.Text = "xrLabel14";
            this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel9.TextFormatString = "{0:N2}";
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel11,
            this.xrLabel16});
            this.ReportFooter.HeightF = 23.46191F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrLabel11
            // 
            this.xrLabel11.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([Amount])")});
            this.xrLabel11.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(546.875F, 0F);
            this.xrLabel11.Multiline = true;
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(103.1249F, 23F);
            this.xrLabel11.StylePriority.UseBorders = false;
            this.xrLabel11.StylePriority.UseFont = false;
            xrSummary2.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.xrLabel11.Summary = xrSummary2;
            this.xrLabel11.Text = "xrLabel14";
            this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel11.TextFormatString = "{0:N2}";
            // 
            // xrLabel16
            // 
            this.xrLabel16.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel16.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel16.Multiline = true;
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel16.SizeF = new System.Drawing.SizeF(546.875F, 23.46191F);
            this.xrLabel16.StylePriority.UseBorders = false;
            this.xrLabel16.StylePriority.UseFont = false;
            this.xrLabel16.StylePriority.UsePadding = false;
            this.xrLabel16.StylePriority.UseTextAlignment = false;
            this.xrLabel16.Text = "Totals";
            this.xrLabel16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine3,
            this.xrLabel17,
            this.xrLabel19,
            this.xrLabel20,
            this.xrLabel21,
            this.xrLine1,
            this.xrLine2,
            this.xrLabel22,
            this.xrLabel23});
            this.PageFooter.Name = "PageFooter";
            // 
            // xrLine3
            // 
            this.xrLine3.LocationFloat = new DevExpress.Utils.PointFloat(1.666689F, 50.04171F);
            this.xrLine3.Name = "xrLine3";
            this.xrLine3.SizeF = new System.Drawing.SizeF(163.9583F, 2F);
            // 
            // xrLabel17
            // 
            this.xrLabel17.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(0F, 52.04169F);
            this.xrLabel17.Multiline = true;
            this.xrLabel17.Name = "xrLabel17";
            this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel17.SizeF = new System.Drawing.SizeF(165.625F, 23F);
            this.xrLabel17.StylePriority.UseFont = false;
            this.xrLabel17.Text = "Prepared By";
            // 
            // xrLabel19
            // 
            this.xrLabel19.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PreparedBy]")});
            this.xrLabel19.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel19.Multiline = true;
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(165.625F, 23F);
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.Text = "xrLabel3";
            // 
            // xrLabel20
            // 
            this.xrLabel20.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CheckedBy]")});
            this.xrLabel20.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(253.1248F, 0F);
            this.xrLabel20.Multiline = true;
            this.xrLabel20.Name = "xrLabel20";
            this.xrLabel20.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel20.SizeF = new System.Drawing.SizeF(165.625F, 23F);
            this.xrLabel20.StylePriority.UseFont = false;
            this.xrLabel20.Text = "xrLabel3";
            // 
            // xrLabel21
            // 
            this.xrLabel21.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(253.1248F, 52.04169F);
            this.xrLabel21.Multiline = true;
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(165.625F, 23F);
            this.xrLabel21.StylePriority.UseFont = false;
            this.xrLabel21.Text = "Checked By";
            // 
            // xrLine1
            // 
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(254.7915F, 50.04171F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(163.9583F, 2F);
            // 
            // xrLine2
            // 
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(486.0417F, 50.04171F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(163.9583F, 2F);
            // 
            // xrLabel22
            // 
            this.xrLabel22.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(484.375F, 52.04169F);
            this.xrLabel22.Multiline = true;
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(165.625F, 23F);
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.Text = "Approved By";
            // 
            // xrLabel23
            // 
            this.xrLabel23.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ApprovedBy]")});
            this.xrLabel23.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel23.LocationFloat = new DevExpress.Utils.PointFloat(484.375F, 0F);
            this.xrLabel23.Multiline = true;
            this.xrLabel23.Name = "xrLabel23";
            this.xrLabel23.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel23.SizeF = new System.Drawing.SizeF(165.625F, 23F);
            this.xrLabel23.StylePriority.UseFont = false;
            this.xrLabel23.Text = "xrLabel3";
            // 
            // TrnOtherIncome
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.Detail,
            this.EmployeeHeader,
            this.EmployeeFooter,
            this.ReportFooter,
            this.PageFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "Query";
            this.DataSource = this.sqlDataSource1;
            this.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.Margins = new DevExpress.Drawing.DXMargins(100F, 100F, 193.125F, 100F);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.ParamId});
            this.Version = "22.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.Parameters.Parameter ParamId;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        private DevExpress.XtraReports.UI.XRLabel xrLabel12;
        private DevExpress.XtraReports.UI.XRLabel xrLabel13;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel39;
        private DevExpress.XtraReports.UI.XRLabel xrLabel18;
        private DevExpress.XtraReports.UI.XRLabel xrLabel15;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel14;
        private DevExpress.XtraReports.UI.GroupHeaderBand EmployeeHeader;
        private DevExpress.XtraReports.UI.GroupFooterBand EmployeeFooter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        private DevExpress.XtraReports.UI.XRLabel xrLabel46;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel16;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.XRLine xrLine3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel17;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
        private DevExpress.XtraReports.UI.XRLabel xrLabel20;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        private DevExpress.XtraReports.UI.XRLabel xrLabel23;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
    }
}
