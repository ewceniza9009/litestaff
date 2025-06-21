namespace whris.UI.Reports
{
    partial class RepPayslip
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepPayslip));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.pageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.ParamPayrollId = new DevExpress.XtraReports.Parameters.Parameter();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel24 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel23 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel26 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel28 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel27 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel30 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel29 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel38 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel37 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel36 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel35 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel34 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel33 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel32 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel40 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel39 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel42 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel41 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel31 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel44 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel43 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel46 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel45 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel47 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine3 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLine4 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel48 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPanel1});
            this.Detail.HeightF = 415.5418F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.SortFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("FullName", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 35.41667F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageInfo2});
            this.BottomMargin.HeightF = 23F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // pageInfo2
            // 
            this.pageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(427.8749F, 0F);
            this.pageInfo2.Name = "pageInfo2";
            this.pageInfo2.SizeF = new System.Drawing.SizeF(325F, 23F);
            this.pageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.pageInfo2.TextFormatString = "Page {0} of {1}";
            // 
            // ParamPayrollId
            // 
            this.ParamPayrollId.Name = "ParamPayrollId";
            this.ParamPayrollId.Type = typeof(int);
            this.ParamPayrollId.ValueInfo = "0";
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "DefaultConnection";
            this.sqlDataSource1.Name = "sqlDataSource1";
            customSqlQuery1.Name = "Query";
            queryParameter1.Name = "ParamPayrollId";
            queryParameter1.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("?ParamPayrollId", typeof(int));
            customSqlQuery1.Parameters.AddRange(new DevExpress.DataAccess.Sql.QueryParameter[] {
            queryParameter1});
            customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            customSqlQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // xrPanel1
            // 
            this.xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1,
            this.xrLabel2,
            this.xrLabel3,
            this.xrLabel4,
            this.xrLabel6,
            this.xrLabel5,
            this.xrLabel8,
            this.xrLabel7,
            this.xrLabel10,
            this.xrLabel9,
            this.xrLabel11,
            this.xrLabel12,
            this.xrLabel14,
            this.xrLabel13,
            this.xrLabel16,
            this.xrLabel15,
            this.xrLabel18,
            this.xrLabel17,
            this.xrLabel20,
            this.xrLabel19,
            this.xrLabel22,
            this.xrLabel21,
            this.xrLabel24,
            this.xrLabel23,
            this.xrLabel26,
            this.xrLabel25,
            this.xrLabel28,
            this.xrLabel27,
            this.xrLabel30,
            this.xrLabel29,
            this.xrLabel38,
            this.xrLabel37,
            this.xrLabel36,
            this.xrLabel35,
            this.xrLabel34,
            this.xrLabel33,
            this.xrLabel32,
            this.xrLabel40,
            this.xrLabel39,
            this.xrLabel42,
            this.xrLabel41,
            this.xrLabel31,
            this.xrLine1,
            this.xrLabel44,
            this.xrLabel43,
            this.xrLabel46,
            this.xrLabel45,
            this.xrLine2,
            this.xrLabel47,
            this.xrLine3,
            this.xrLine4,
            this.xrLabel48});
            this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPanel1.Name = "xrPanel1";
            this.xrPanel1.SizeF = new System.Drawing.SizeF(755F, 415.5418F);
            // 
            // xrLabel1
            // 
            this.xrLabel1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[FullName]")});
            this.xrLabel1.Font = new DevExpress.Drawing.DXFont("Arial", 12F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(639.3751F, 23F);
            this.xrLabel1.StylePriority.UseBorders = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.Text = "xrLabel1";
            // 
            // xrLabel2
            // 
            this.xrLabel2.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NetIncome]")});
            this.xrLabel2.Font = new DevExpress.Drawing.DXFont("Arial", 12F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(639.3751F, 0F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(115.6249F, 23F);
            this.xrLabel2.StylePriority.UseBorders = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "xrLabel1";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel2.TextFormatString = "{0:N2}";
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Company]")});
            this.xrLabel3.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(122.6667F, 33.29166F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(145.5833F, 23F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "xrLabel3";
            // 
            // xrLabel4
            // 
            this.xrLabel4.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(2.083349F, 33.29166F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(119.7917F, 23F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.Text = "Company:";
            // 
            // xrLabel6
            // 
            this.xrLabel6.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(2.874978F, 56.29165F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(119.7917F, 23F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.Text = "Payroll Number:";
            // 
            // xrLabel5
            // 
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PayrollNumber]")});
            this.xrLabel5.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(122.6667F, 56.29165F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(145.5833F, 23F);
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.Text = "xrLabel3";
            // 
            // xrLabel8
            // 
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PayrollDate]")});
            this.xrLabel8.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(122.6667F, 79.29166F);
            this.xrLabel8.Multiline = true;
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(145.5833F, 23.00001F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.Text = "xrLabel3";
            this.xrLabel8.TextFormatString = "{0:d}";
            // 
            // xrLabel7
            // 
            this.xrLabel7.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(2.874978F, 79.29166F);
            this.xrLabel7.Multiline = true;
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(119.7917F, 23F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.Text = "Payroll Date:";
            // 
            // xrLabel10
            // 
            this.xrLabel10.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(2.874978F, 102.2917F);
            this.xrLabel10.Multiline = true;
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(119.7917F, 23F);
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.Text = "Remarks:";
            // 
            // xrLabel9
            // 
            this.xrLabel9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Remarks]")});
            this.xrLabel9.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(122.6667F, 102.2917F);
            this.xrLabel9.Multiline = true;
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(145.5833F, 23.00001F);
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.Text = "xrLabel3";
            // 
            // xrLabel11
            // 
            this.xrLabel11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BasicSalary]")});
            this.xrLabel11.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(183.0833F, 135.375F);
            this.xrLabel11.Multiline = true;
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(84.37505F, 22.99997F);
            this.xrLabel11.StylePriority.UseFont = false;
            this.xrLabel11.Text = "xrLabel11";
            this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel11.TextFormatString = "{0:N2}";
            // 
            // xrLabel12
            // 
            this.xrLabel12.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(2.083349F, 135.375F);
            this.xrLabel12.Multiline = true;
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(181F, 22.99997F);
            this.xrLabel12.StylePriority.UseFont = false;
            this.xrLabel12.Text = "Basic Salary:";
            // 
            // xrLabel14
            // 
            this.xrLabel14.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(2.083333F, 158.375F);
            this.xrLabel14.Multiline = true;
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(181F, 23.00003F);
            this.xrLabel14.StylePriority.UseFont = false;
            this.xrLabel14.Text = "Other Salary (OT/Holiday):";
            // 
            // xrLabel13
            // 
            this.xrLabel13.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OtherSalary]")});
            this.xrLabel13.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(183.0833F, 158.375F);
            this.xrLabel13.Multiline = true;
            this.xrLabel13.Name = "xrLabel13";
            this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel13.SizeF = new System.Drawing.SizeF(84.37505F, 23.00003F);
            this.xrLabel13.StylePriority.UseFont = false;
            this.xrLabel13.Text = "xrLabel11";
            this.xrLabel13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel13.TextFormatString = "{0:N2}";
            // 
            // xrLabel16
            // 
            this.xrLabel16.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalSalaryAmount]")});
            this.xrLabel16.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(183.0833F, 181.375F);
            this.xrLabel16.Multiline = true;
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel16.SizeF = new System.Drawing.SizeF(84.37505F, 23.00002F);
            this.xrLabel16.StylePriority.UseFont = false;
            this.xrLabel16.Text = "xrLabel11";
            this.xrLabel16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel16.TextFormatString = "{0:N2}";
            // 
            // xrLabel15
            // 
            this.xrLabel15.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(2.083325F, 181.375F);
            this.xrLabel15.Multiline = true;
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(181F, 23.00002F);
            this.xrLabel15.StylePriority.UseFont = false;
            this.xrLabel15.Text = "Total Salary:";
            // 
            // xrLabel18
            // 
            this.xrLabel18.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(2.874978F, 204.375F);
            this.xrLabel18.Multiline = true;
            this.xrLabel18.Name = "xrLabel18";
            this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel18.SizeF = new System.Drawing.SizeF(180.2083F, 23.00002F);
            this.xrLabel18.StylePriority.UseFont = false;
            this.xrLabel18.Text = "LESS: Tardy";
            // 
            // xrLabel17
            // 
            this.xrLabel17.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalTardyAmount]")});
            this.xrLabel17.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(183.0833F, 204.375F);
            this.xrLabel17.Multiline = true;
            this.xrLabel17.Name = "xrLabel17";
            this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel17.SizeF = new System.Drawing.SizeF(85.16673F, 23.00002F);
            this.xrLabel17.StylePriority.UseFont = false;
            this.xrLabel17.Text = "xrLabel11";
            this.xrLabel17.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel17.TextFormatString = "{0:N2}";
            // 
            // xrLabel20
            // 
            this.xrLabel20.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalAbsentAmount]")});
            this.xrLabel20.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(183.0833F, 227.375F);
            this.xrLabel20.Multiline = true;
            this.xrLabel20.Name = "xrLabel20";
            this.xrLabel20.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel20.SizeF = new System.Drawing.SizeF(85.16673F, 23.00002F);
            this.xrLabel20.StylePriority.UseFont = false;
            this.xrLabel20.Text = "xrLabel11";
            this.xrLabel20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel20.TextFormatString = "{0:N2}";
            // 
            // xrLabel19
            // 
            this.xrLabel19.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(2.87497F, 227.375F);
            this.xrLabel19.Multiline = true;
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(180.2083F, 23.00002F);
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.Text = "           Absent";
            // 
            // xrLabel22
            // 
            this.xrLabel22.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(2.874986F, 250.375F);
            this.xrLabel22.Multiline = true;
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(180.2083F, 23.00002F);
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.Text = "Net Salary:";
            // 
            // xrLabel21
            // 
            this.xrLabel21.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalNetSalaryAmount]")});
            this.xrLabel21.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(183.0833F, 250.375F);
            this.xrLabel21.Multiline = true;
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(85.16673F, 23.00002F);
            this.xrLabel21.StylePriority.UseFont = false;
            this.xrLabel21.Text = "xrLabel11";
            this.xrLabel21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel21.TextFormatString = "{0:N2}";
            // 
            // xrLabel24
            // 
            this.xrLabel24.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalOtherIncomeTaxable]")});
            this.xrLabel24.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel24.LocationFloat = new DevExpress.Utils.PointFloat(183.0833F, 273.3751F);
            this.xrLabel24.Multiline = true;
            this.xrLabel24.Name = "xrLabel24";
            this.xrLabel24.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel24.SizeF = new System.Drawing.SizeF(85.16673F, 23.00003F);
            this.xrLabel24.StylePriority.UseFont = false;
            this.xrLabel24.Text = "xrLabel11";
            this.xrLabel24.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel24.TextFormatString = "{0:N2}";
            // 
            // xrLabel23
            // 
            this.xrLabel23.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel23.LocationFloat = new DevExpress.Utils.PointFloat(2.874994F, 273.3751F);
            this.xrLabel23.Multiline = true;
            this.xrLabel23.Name = "xrLabel23";
            this.xrLabel23.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel23.SizeF = new System.Drawing.SizeF(180.2083F, 23.00003F);
            this.xrLabel23.StylePriority.UseFont = false;
            this.xrLabel23.Text = "Other Income (Taxable):";
            // 
            // xrLabel26
            // 
            this.xrLabel26.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel26.LocationFloat = new DevExpress.Utils.PointFloat(2.87497F, 296.3751F);
            this.xrLabel26.Multiline = true;
            this.xrLabel26.Name = "xrLabel26";
            this.xrLabel26.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel26.SizeF = new System.Drawing.SizeF(180.2083F, 23.00003F);
            this.xrLabel26.StylePriority.UseFont = false;
            this.xrLabel26.Text = "Income";
            // 
            // xrLabel25
            // 
            this.xrLabel25.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[GrossIncome]")});
            this.xrLabel25.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel25.LocationFloat = new DevExpress.Utils.PointFloat(183.0833F, 296.3751F);
            this.xrLabel25.Multiline = true;
            this.xrLabel25.Name = "xrLabel25";
            this.xrLabel25.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel25.SizeF = new System.Drawing.SizeF(85.16673F, 23.00003F);
            this.xrLabel25.StylePriority.UseFont = false;
            this.xrLabel25.Text = "xrLabel11";
            this.xrLabel25.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel25.TextFormatString = "{0:N2}";
            // 
            // xrLabel28
            // 
            this.xrLabel28.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalOtherIncomeNonTaxable]")});
            this.xrLabel28.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel28.LocationFloat = new DevExpress.Utils.PointFloat(183.0833F, 319.3751F);
            this.xrLabel28.Multiline = true;
            this.xrLabel28.Name = "xrLabel28";
            this.xrLabel28.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel28.SizeF = new System.Drawing.SizeF(85.16673F, 23.00003F);
            this.xrLabel28.StylePriority.UseFont = false;
            this.xrLabel28.Text = "xrLabel11";
            this.xrLabel28.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel28.TextFormatString = "{0:N2}";
            // 
            // xrLabel27
            // 
            this.xrLabel27.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel27.LocationFloat = new DevExpress.Utils.PointFloat(2.874954F, 319.3751F);
            this.xrLabel27.Multiline = true;
            this.xrLabel27.Name = "xrLabel27";
            this.xrLabel27.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel27.SizeF = new System.Drawing.SizeF(180.2083F, 23.00003F);
            this.xrLabel27.StylePriority.UseFont = false;
            this.xrLabel27.Text = "Other Income (Non - taxable):";
            // 
            // xrLabel30
            // 
            this.xrLabel30.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel30.LocationFloat = new DevExpress.Utils.PointFloat(2.874947F, 342.3751F);
            this.xrLabel30.Multiline = true;
            this.xrLabel30.Name = "xrLabel30";
            this.xrLabel30.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel30.SizeF = new System.Drawing.SizeF(180.2083F, 23.00003F);
            this.xrLabel30.StylePriority.UseFont = false;
            this.xrLabel30.Text = "Gross Income:";
            // 
            // xrLabel29
            // 
            this.xrLabel29.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalOtherIncomeNonTaxable]")});
            this.xrLabel29.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel29.LocationFloat = new DevExpress.Utils.PointFloat(183.0833F, 342.3751F);
            this.xrLabel29.Multiline = true;
            this.xrLabel29.Name = "xrLabel29";
            this.xrLabel29.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel29.SizeF = new System.Drawing.SizeF(85.16673F, 23.00003F);
            this.xrLabel29.StylePriority.UseFont = false;
            this.xrLabel29.Text = "xrLabel11";
            this.xrLabel29.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel29.TextFormatString = "{0:N2}";
            // 
            // xrLabel38
            // 
            this.xrLabel38.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[HDMFContribution]")});
            this.xrLabel38.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel38.LocationFloat = new DevExpress.Utils.PointFloat(463.9167F, 102.2917F);
            this.xrLabel38.Multiline = true;
            this.xrLabel38.Name = "xrLabel38";
            this.xrLabel38.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel38.SizeF = new System.Drawing.SizeF(97.6667F, 23.00002F);
            this.xrLabel38.StylePriority.UseFont = false;
            this.xrLabel38.Text = "xrLabel11";
            this.xrLabel38.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel38.TextFormatString = "{0:N2}";
            // 
            // xrLabel37
            // 
            this.xrLabel37.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel37.LocationFloat = new DevExpress.Utils.PointFloat(280.5833F, 102.2917F);
            this.xrLabel37.Multiline = true;
            this.xrLabel37.Name = "xrLabel37";
            this.xrLabel37.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel37.SizeF = new System.Drawing.SizeF(183.3334F, 23.00002F);
            this.xrLabel37.StylePriority.UseFont = false;
            this.xrLabel37.Text = "HDMF:";
            // 
            // xrLabel36
            // 
            this.xrLabel36.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel36.LocationFloat = new DevExpress.Utils.PointFloat(280.5833F, 79.29163F);
            this.xrLabel36.Multiline = true;
            this.xrLabel36.Name = "xrLabel36";
            this.xrLabel36.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel36.SizeF = new System.Drawing.SizeF(183.3334F, 23.00003F);
            this.xrLabel36.StylePriority.UseFont = false;
            this.xrLabel36.Text = "PHIC:";
            // 
            // xrLabel35
            // 
            this.xrLabel35.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PHICContribution]")});
            this.xrLabel35.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel35.LocationFloat = new DevExpress.Utils.PointFloat(463.9167F, 79.29163F);
            this.xrLabel35.Multiline = true;
            this.xrLabel35.Name = "xrLabel35";
            this.xrLabel35.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel35.SizeF = new System.Drawing.SizeF(97.6667F, 23.00002F);
            this.xrLabel35.StylePriority.UseFont = false;
            this.xrLabel35.Text = "xrLabel11";
            this.xrLabel35.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel35.TextFormatString = "{0:N2}";
            // 
            // xrLabel34
            // 
            this.xrLabel34.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SSSContribution]")});
            this.xrLabel34.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel34.LocationFloat = new DevExpress.Utils.PointFloat(463.9167F, 56.29165F);
            this.xrLabel34.Multiline = true;
            this.xrLabel34.Name = "xrLabel34";
            this.xrLabel34.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel34.SizeF = new System.Drawing.SizeF(97.6667F, 23.00002F);
            this.xrLabel34.StylePriority.UseFont = false;
            this.xrLabel34.Text = "xrLabel11";
            this.xrLabel34.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel34.TextFormatString = "{0:N2}";
            // 
            // xrLabel33
            // 
            this.xrLabel33.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel33.LocationFloat = new DevExpress.Utils.PointFloat(280.5833F, 56.29165F);
            this.xrLabel33.Multiline = true;
            this.xrLabel33.Name = "xrLabel33";
            this.xrLabel33.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel33.SizeF = new System.Drawing.SizeF(183.3334F, 23.00002F);
            this.xrLabel33.StylePriority.UseFont = false;
            this.xrLabel33.Text = "SSS Contribution:";
            // 
            // xrLabel32
            // 
            this.xrLabel32.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel32.LocationFloat = new DevExpress.Utils.PointFloat(280.5834F, 33.29166F);
            this.xrLabel32.Multiline = true;
            this.xrLabel32.Name = "xrLabel32";
            this.xrLabel32.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel32.SizeF = new System.Drawing.SizeF(183.3333F, 23.00002F);
            this.xrLabel32.StylePriority.UseFont = false;
            this.xrLabel32.Text = "Mandatory Deductions";
            // 
            // xrLabel40
            // 
            this.xrLabel40.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel40.LocationFloat = new DevExpress.Utils.PointFloat(280.5834F, 125.2917F);
            this.xrLabel40.Multiline = true;
            this.xrLabel40.Name = "xrLabel40";
            this.xrLabel40.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel40.SizeF = new System.Drawing.SizeF(183.3333F, 23.00002F);
            this.xrLabel40.StylePriority.UseFont = false;
            this.xrLabel40.Text = "Withholding Tax:";
            // 
            // xrLabel39
            // 
            this.xrLabel39.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Tax]")});
            this.xrLabel39.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel39.LocationFloat = new DevExpress.Utils.PointFloat(463.9167F, 125.2917F);
            this.xrLabel39.Multiline = true;
            this.xrLabel39.Name = "xrLabel39";
            this.xrLabel39.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel39.SizeF = new System.Drawing.SizeF(97.6667F, 23.00002F);
            this.xrLabel39.StylePriority.UseFont = false;
            this.xrLabel39.Text = "xrLabel11";
            this.xrLabel39.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel39.TextFormatString = "{0:N2}";
            // 
            // xrLabel42
            // 
            this.xrLabel42.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel42.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalDeduction]")});
            this.xrLabel42.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel42.LocationFloat = new DevExpress.Utils.PointFloat(463.9167F, 148.2917F);
            this.xrLabel42.Multiline = true;
            this.xrLabel42.Name = "xrLabel42";
            this.xrLabel42.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel42.SizeF = new System.Drawing.SizeF(97.6667F, 23.00002F);
            this.xrLabel42.StylePriority.UseBorders = false;
            this.xrLabel42.StylePriority.UseFont = false;
            this.xrLabel42.Text = "xrLabel11";
            this.xrLabel42.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel42.TextFormatString = "{0:N2}";
            // 
            // xrLabel41
            // 
            this.xrLabel41.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel41.LocationFloat = new DevExpress.Utils.PointFloat(280.5834F, 148.2917F);
            this.xrLabel41.Multiline = true;
            this.xrLabel41.Name = "xrLabel41";
            this.xrLabel41.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel41.SizeF = new System.Drawing.SizeF(183.3333F, 23.00002F);
            this.xrLabel41.StylePriority.UseFont = false;
            this.xrLabel41.Text = "Mandatory Deductions Total:";
            // 
            // xrLabel31
            // 
            this.xrLabel31.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel31.LocationFloat = new DevExpress.Utils.PointFloat(463.9168F, 33.29166F);
            this.xrLabel31.Multiline = true;
            this.xrLabel31.Name = "xrLabel31";
            this.xrLabel31.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel31.SizeF = new System.Drawing.SizeF(97.66666F, 23.00002F);
            this.xrLabel31.StylePriority.UseFont = false;
            this.xrLabel31.StylePriority.UseTextAlignment = false;
            this.xrLabel31.Text = "Amount";
            this.xrLabel31.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrLine1
            // 
            this.xrLine1.LineStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 390.3751F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(745F, 2.000031F);
            // 
            // xrLabel44
            // 
            this.xrLabel44.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel44.LocationFloat = new DevExpress.Utils.PointFloat(280.5833F, 181.375F);
            this.xrLabel44.Multiline = true;
            this.xrLabel44.Name = "xrLabel44";
            this.xrLabel44.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel44.SizeF = new System.Drawing.SizeF(183.3334F, 23.00002F);
            this.xrLabel44.StylePriority.UseFont = false;
            this.xrLabel44.Text = "Other Deductions";
            // 
            // xrLabel43
            // 
            this.xrLabel43.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel43.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalOtherDeduction]")});
            this.xrLabel43.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel43.LocationFloat = new DevExpress.Utils.PointFloat(463.9167F, 181.375F);
            this.xrLabel43.Multiline = true;
            this.xrLabel43.Name = "xrLabel43";
            this.xrLabel43.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel43.SizeF = new System.Drawing.SizeF(97.6667F, 23.00002F);
            this.xrLabel43.StylePriority.UseBorders = false;
            this.xrLabel43.StylePriority.UseFont = false;
            this.xrLabel43.Text = "xrLabel11";
            this.xrLabel43.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel43.TextFormatString = "{0:N2}";
            // 
            // xrLabel46
            // 
            this.xrLabel46.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Double;
            this.xrLabel46.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrLabel46.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NetIncome]")});
            this.xrLabel46.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel46.LocationFloat = new DevExpress.Utils.PointFloat(463.9167F, 214.875F);
            this.xrLabel46.Multiline = true;
            this.xrLabel46.Name = "xrLabel46";
            this.xrLabel46.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel46.SizeF = new System.Drawing.SizeF(97.6667F, 23.00002F);
            this.xrLabel46.StylePriority.UseBorderDashStyle = false;
            this.xrLabel46.StylePriority.UseBorders = false;
            this.xrLabel46.StylePriority.UseFont = false;
            this.xrLabel46.Text = "xrLabel11";
            this.xrLabel46.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel46.TextFormatString = "{0:N2}";
            // 
            // xrLabel45
            // 
            this.xrLabel45.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel45.LocationFloat = new DevExpress.Utils.PointFloat(280.5834F, 214.875F);
            this.xrLabel45.Multiline = true;
            this.xrLabel45.Name = "xrLabel45";
            this.xrLabel45.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel45.SizeF = new System.Drawing.SizeF(183.3333F, 23.00002F);
            this.xrLabel45.StylePriority.UseFont = false;
            this.xrLabel45.Text = "Net Pay:";
            // 
            // xrLine2
            // 
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(463.9168F, 239.875F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(97.66672F, 2F);
            // 
            // xrLabel47
            // 
            this.xrLabel47.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel47.LocationFloat = new DevExpress.Utils.PointFloat(589.375F, 102.2917F);
            this.xrLabel47.Multiline = true;
            this.xrLabel47.Name = "xrLabel47";
            this.xrLabel47.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel47.SizeF = new System.Drawing.SizeF(165.625F, 23F);
            this.xrLabel47.StylePriority.UseFont = false;
            this.xrLabel47.Text = "Prepared By:";
            // 
            // xrLine3
            // 
            this.xrLine3.LocationFloat = new DevExpress.Utils.PointFloat(591.0417F, 152.3335F);
            this.xrLine3.Name = "xrLine3";
            this.xrLine3.SizeF = new System.Drawing.SizeF(163.9583F, 2F);
            // 
            // xrLine4
            // 
            this.xrLine4.LocationFloat = new DevExpress.Utils.PointFloat(591.0417F, 273.3751F);
            this.xrLine4.Name = "xrLine4";
            this.xrLine4.SizeF = new System.Drawing.SizeF(163.9583F, 2F);
            // 
            // xrLabel48
            // 
            this.xrLabel48.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel48.LocationFloat = new DevExpress.Utils.PointFloat(589.375F, 223.3333F);
            this.xrLabel48.Multiline = true;
            this.xrLabel48.Name = "xrLabel48";
            this.xrLabel48.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel48.SizeF = new System.Drawing.SizeF(165.625F, 23F);
            this.xrLabel48.StylePriority.UseFont = false;
            this.xrLabel48.Text = "Received By:";
            // 
            // RepPayslip
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "Query";
            this.DataSource = this.sqlDataSource1;
            this.Margins = new DevExpress.Drawing.DXMargins(44F, 51F, 35.41667F, 23F);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.ParamPayrollId});
            this.Version = "22.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.Parameters.Parameter ParamPayrollId;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.XRPanel xrPanel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel12;
        private DevExpress.XtraReports.UI.XRLabel xrLabel14;
        private DevExpress.XtraReports.UI.XRLabel xrLabel13;
        private DevExpress.XtraReports.UI.XRLabel xrLabel16;
        private DevExpress.XtraReports.UI.XRLabel xrLabel15;
        private DevExpress.XtraReports.UI.XRLabel xrLabel18;
        private DevExpress.XtraReports.UI.XRLabel xrLabel17;
        private DevExpress.XtraReports.UI.XRLabel xrLabel20;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLabel xrLabel24;
        private DevExpress.XtraReports.UI.XRLabel xrLabel23;
        private DevExpress.XtraReports.UI.XRLabel xrLabel26;
        private DevExpress.XtraReports.UI.XRLabel xrLabel25;
        private DevExpress.XtraReports.UI.XRLabel xrLabel28;
        private DevExpress.XtraReports.UI.XRLabel xrLabel27;
        private DevExpress.XtraReports.UI.XRLabel xrLabel30;
        private DevExpress.XtraReports.UI.XRLabel xrLabel29;
        private DevExpress.XtraReports.UI.XRLabel xrLabel38;
        private DevExpress.XtraReports.UI.XRLabel xrLabel37;
        private DevExpress.XtraReports.UI.XRLabel xrLabel36;
        private DevExpress.XtraReports.UI.XRLabel xrLabel35;
        private DevExpress.XtraReports.UI.XRLabel xrLabel34;
        private DevExpress.XtraReports.UI.XRLabel xrLabel33;
        private DevExpress.XtraReports.UI.XRLabel xrLabel32;
        private DevExpress.XtraReports.UI.XRLabel xrLabel40;
        private DevExpress.XtraReports.UI.XRLabel xrLabel39;
        private DevExpress.XtraReports.UI.XRLabel xrLabel42;
        private DevExpress.XtraReports.UI.XRLabel xrLabel41;
        private DevExpress.XtraReports.UI.XRLabel xrLabel31;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel44;
        private DevExpress.XtraReports.UI.XRLabel xrLabel43;
        private DevExpress.XtraReports.UI.XRLabel xrLabel46;
        private DevExpress.XtraReports.UI.XRLabel xrLabel45;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel47;
        private DevExpress.XtraReports.UI.XRLine xrLine3;
        private DevExpress.XtraReports.UI.XRLine xrLine4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel48;
    }
}
