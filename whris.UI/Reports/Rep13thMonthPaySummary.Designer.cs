namespace whris.UI.Reports
{
    partial class Rep13thMonthPaySummary
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
            DevExpress.DataAccess.Sql.QueryParameter queryParameter5 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter6 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter7 = new DevExpress.DataAccess.Sql.QueryParameter();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Rep13thMonthPaySummary));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.ParamStartPayNo = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamEndPayNo = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamEmployeeId = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamPayrollGroupId = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamCompanyId = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamBranchId = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamDepartmentId = new DevExpress.XtraReports.Parameters.Parameter();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel5,
            this.xrLabel8,
            this.xrLabel6});
            this.Detail.HeightF = 31.61784F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4,
            this.xrLabel2,
            this.xrLabel3,
            this.xrLabel1,
            this.xrLabel11,
            this.xrLabel13,
            this.xrLabel14});
            this.TopMargin.HeightF = 191.6667F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Font = new DevExpress.Drawing.DXFont("Arial", 12F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(42.70833F, 24.58334F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(662.6484F, 23F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "13th Month Pay Report Summary";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel11
            // 
            this.xrLabel11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Company \' + [Company]")});
            this.xrLabel11.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(0F, 68.50002F);
            this.xrLabel11.Multiline = true;
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(650F, 23F);
            this.xrLabel11.StylePriority.UseFont = false;
            // 
            // xrLabel13
            // 
            this.xrLabel13.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Branch \' + [Branch]")});
            this.xrLabel13.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(0F, 91.50003F);
            this.xrLabel13.Multiline = true;
            this.xrLabel13.Name = "xrLabel13";
            this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel13.SizeF = new System.Drawing.SizeF(650F, 23F);
            this.xrLabel13.StylePriority.UseFont = false;
            // 
            // xrLabel14
            // 
            this.xrLabel14.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Department \' + [Department]")});
            this.xrLabel14.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(0F, 114.5F);
            this.xrLabel14.Multiline = true;
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(650F, 23F);
            this.xrLabel14.StylePriority.UseFont = false;
            // 
            // ParamStartPayNo
            // 
            this.ParamStartPayNo.Name = "ParamStartPayNo";
            this.ParamStartPayNo.Type = typeof(int);
            this.ParamStartPayNo.ValueInfo = "0";
            // 
            // ParamEndPayNo
            // 
            this.ParamEndPayNo.Name = "ParamEndPayNo";
            this.ParamEndPayNo.Type = typeof(int);
            this.ParamEndPayNo.ValueInfo = "0";
            // 
            // ParamEmployeeId
            // 
            this.ParamEmployeeId.Name = "ParamEmployeeId";
            this.ParamEmployeeId.Type = typeof(int);
            this.ParamEmployeeId.ValueInfo = "0";
            // 
            // ParamPayrollGroupId
            // 
            this.ParamPayrollGroupId.Name = "ParamPayrollGroupId";
            this.ParamPayrollGroupId.Type = typeof(int);
            this.ParamPayrollGroupId.ValueInfo = "0";
            // 
            // ParamCompanyId
            // 
            this.ParamCompanyId.Name = "ParamCompanyId";
            this.ParamCompanyId.Type = typeof(int);
            this.ParamCompanyId.ValueInfo = "0";
            // 
            // ParamBranchId
            // 
            this.ParamBranchId.Name = "ParamBranchId";
            this.ParamBranchId.Type = typeof(int);
            this.ParamBranchId.ValueInfo = "0";
            // 
            // ParamDepartmentId
            // 
            this.ParamDepartmentId.Name = "ParamDepartmentId";
            this.ParamDepartmentId.Type = typeof(int);
            this.ParamDepartmentId.ValueInfo = "0";
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "DefaultConnection";
            this.sqlDataSource1.Name = "sqlDataSource1";
            customSqlQuery1.Name = "Query";
            queryParameter1.Name = "ParamStartPayNo";
            queryParameter1.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("?ParamStartPayNo", typeof(int));
            queryParameter2.Name = "ParamEndPayNo";
            queryParameter2.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter2.Value = new DevExpress.DataAccess.Expression("?ParamEndPayNo", typeof(int));
            queryParameter3.Name = "ParamEmployeeId";
            queryParameter3.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter3.Value = new DevExpress.DataAccess.Expression("?ParamEmployeeId", typeof(int));
            queryParameter4.Name = "ParamPayrollGroupId";
            queryParameter4.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter4.Value = new DevExpress.DataAccess.Expression("?ParamPayrollGroupId", typeof(int));
            queryParameter5.Name = "ParamCompanyId";
            queryParameter5.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter5.Value = new DevExpress.DataAccess.Expression("?ParamCompanyId", typeof(int));
            queryParameter6.Name = "ParamBranchId";
            queryParameter6.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter6.Value = new DevExpress.DataAccess.Expression("?ParamBranchId", typeof(int));
            queryParameter7.Name = "ParamDepartmentId";
            queryParameter7.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter7.Value = new DevExpress.DataAccess.Expression("?ParamDepartmentId", typeof(int));
            customSqlQuery1.Parameters.AddRange(new DevExpress.DataAccess.Sql.QueryParameter[] {
            queryParameter1,
            queryParameter2,
            queryParameter3,
            queryParameter4,
            queryParameter5,
            queryParameter6,
            queryParameter7});
            customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            customSqlQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // xrLabel2
            // 
            this.xrLabel2.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel2.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(319.5833F, 164.2155F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(190.2826F, 27.45117F);
            this.xrLabel2.StylePriority.UseBorders = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UsePadding = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Total";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel4
            // 
            this.xrLabel4.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel4.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(509.8658F, 164.2155F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(221.5326F, 27.45117F);
            this.xrLabel4.StylePriority.UseBorders = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UsePadding = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.Text = "13th Month Pay";
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel3
            // 
            this.xrLabel3.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel3.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(42.70833F, 164.2155F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(276.8749F, 27.45117F);
            this.xrLabel3.StylePriority.UseBorders = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UsePadding = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "EmployeeName";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel8
            // 
            this.xrLabel8.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalWorkingAmount]")});
            this.xrLabel8.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(319.5834F, 0F);
            this.xrLabel8.Multiline = true;
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(190.2826F, 31.61784F);
            this.xrLabel8.StylePriority.UseBorders = false;
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UsePadding = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "Payroll";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            this.xrLabel8.TextFormatString = "{0:N2}";
            // 
            // xrLabel6
            // 
            this.xrLabel6.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[FullName]")});
            this.xrLabel6.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(42.70833F, 0F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(276.8749F, 31.61784F);
            this.xrLabel6.StylePriority.UseBorders = false;
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UsePadding = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "Payroll";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel5
            // 
            this.xrLabel5.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalWorkingAmount] / 12")});
            this.xrLabel5.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(509.8658F, 0F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(221.5326F, 31.61784F);
            this.xrLabel5.StylePriority.UseBorders = false;
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UsePadding = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "Payroll";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            this.xrLabel5.TextFormatString = "{0:N2}";
            // 
            // Rep13thMonthPaySummary
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "Query";
            this.DataSource = this.sqlDataSource1;
            this.Margins = new DevExpress.Drawing.DXMargins(27F, 33F, 191.6667F, 100F);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.ParamStartPayNo,
            this.ParamEndPayNo,
            this.ParamEmployeeId,
            this.ParamPayrollGroupId,
            this.ParamCompanyId,
            this.ParamBranchId,
            this.ParamDepartmentId});
            this.Version = "22.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel13;
        private DevExpress.XtraReports.UI.XRLabel xrLabel14;
        private DevExpress.XtraReports.Parameters.Parameter ParamStartPayNo;
        private DevExpress.XtraReports.Parameters.Parameter ParamEndPayNo;
        private DevExpress.XtraReports.Parameters.Parameter ParamEmployeeId;
        private DevExpress.XtraReports.Parameters.Parameter ParamPayrollGroupId;
        private DevExpress.XtraReports.Parameters.Parameter ParamCompanyId;
        private DevExpress.XtraReports.Parameters.Parameter ParamBranchId;
        private DevExpress.XtraReports.Parameters.Parameter ParamDepartmentId;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
    }
}
