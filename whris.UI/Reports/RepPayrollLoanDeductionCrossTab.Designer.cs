namespace whris.UI.Reports
{
    partial class RepPayrollLoanDeductionCrossTab
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
            DevExpress.XtraReports.UI.CrossTab.CrossTabColumnField crossTabColumnField1 = new DevExpress.XtraReports.UI.CrossTab.CrossTabColumnField();
            DevExpress.XtraReports.UI.CrossTab.CrossTabDataField crossTabDataField1 = new DevExpress.XtraReports.UI.CrossTab.CrossTabDataField();
            DevExpress.XtraReports.UI.CrossTab.CrossTabRowDefinition crossTabRowDefinition1 = new DevExpress.XtraReports.UI.CrossTab.CrossTabRowDefinition(40F);
            DevExpress.XtraReports.UI.CrossTab.CrossTabRowField crossTabRowField1 = new DevExpress.XtraReports.UI.CrossTab.CrossTabRowField();
            DevExpress.XtraReports.UI.CrossTab.CrossTabRowField crossTabRowField2 = new DevExpress.XtraReports.UI.CrossTab.CrossTabRowField();
            DevExpress.DataAccess.Sql.CustomSqlQuery customSqlQuery1 = new DevExpress.DataAccess.Sql.CustomSqlQuery();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter2 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter3 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter4 = new DevExpress.DataAccess.Sql.QueryParameter();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepPayrollLoanDeductionCrossTab));
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrCrossTab1 = new DevExpress.XtraReports.UI.XRCrossTab();
            this.crossTabHeaderCell1 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabDataCell1 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabHeaderCell2 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabHeaderCell3 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabHeaderCell4 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabTotalCell1 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabHeaderCell5 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabTotalCell2 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabTotalCell3 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabHeaderCell6 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabHeaderCell7 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabHeaderCell8 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabTotalCell4 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.crossTabTotalCell5 = new DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.crossTabGeneralStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.crossTabHeaderStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.crossTabDataStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.crossTabTotalStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.ParamPayrollId = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamDateStart = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamDateEnd = new DevExpress.XtraReports.Parameters.Parameter();
            this.ParamPayrollNumber = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.xrCrossTab1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrCrossTab1});
            this.Detail.HeightF = 160F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrCrossTab1
            // 
            this.xrCrossTab1.Cells.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.crossTabHeaderCell1,
            this.crossTabDataCell1,
            this.crossTabHeaderCell2,
            this.crossTabHeaderCell3,
            this.crossTabHeaderCell4,
            this.crossTabTotalCell1,
            this.crossTabHeaderCell5,
            this.crossTabTotalCell2,
            this.crossTabTotalCell3,
            this.crossTabHeaderCell6,
            this.crossTabHeaderCell7,
            this.crossTabHeaderCell8,
            this.crossTabTotalCell4,
            this.crossTabTotalCell5});
            this.xrCrossTab1.ColumnDefinitions.AddRange(new DevExpress.XtraReports.UI.CrossTab.CrossTabColumnDefinition[] {
            new DevExpress.XtraReports.UI.CrossTab.CrossTabColumnDefinition(65.62499F),
            new DevExpress.XtraReports.UI.CrossTab.CrossTabColumnDefinition(129.6875F),
            new DevExpress.XtraReports.UI.CrossTab.CrossTabColumnDefinition(75.52084F),
            new DevExpress.XtraReports.UI.CrossTab.CrossTabColumnDefinition(81.77077F)});
            crossTabColumnField1.FieldName = "OtherDeduction";
            this.xrCrossTab1.ColumnFields.AddRange(new DevExpress.XtraReports.UI.CrossTab.CrossTabColumnField[] {
            crossTabColumnField1});
            this.xrCrossTab1.DataAreaStyleName = "crossTabDataStyle1";
            crossTabDataField1.FieldName = "DeductionAmount";
            this.xrCrossTab1.DataFields.AddRange(new DevExpress.XtraReports.UI.CrossTab.CrossTabDataField[] {
            crossTabDataField1});
            this.xrCrossTab1.DataMember = "Query";
            this.xrCrossTab1.DataSource = this.sqlDataSource1;
            this.xrCrossTab1.GeneralStyleName = "crossTabGeneralStyle1";
            this.xrCrossTab1.HeaderAreaStyleName = "crossTabHeaderStyle1";
            this.xrCrossTab1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrCrossTab1.Name = "xrCrossTab1";
            crossTabRowDefinition1.AutoHeightMode = DevExpress.XtraReports.UI.AutoSizeMode.ShrinkAndGrow;
            this.xrCrossTab1.RowDefinitions.AddRange(new DevExpress.XtraReports.UI.CrossTab.CrossTabRowDefinition[] {
            new DevExpress.XtraReports.UI.CrossTab.CrossTabRowDefinition(40F),
            crossTabRowDefinition1,
            new DevExpress.XtraReports.UI.CrossTab.CrossTabRowDefinition(40F),
            new DevExpress.XtraReports.UI.CrossTab.CrossTabRowDefinition(40F)});
            crossTabRowField1.FieldName = "PayrollGroup";
            crossTabRowField2.FieldName = "Employee";
            this.xrCrossTab1.RowFields.AddRange(new DevExpress.XtraReports.UI.CrossTab.CrossTabRowField[] {
            crossTabRowField1,
            crossTabRowField2});
            this.xrCrossTab1.SizeF = new System.Drawing.SizeF(352.6041F, 160F);
            this.xrCrossTab1.SnapLineMargin = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 10, 10, 100F);
            this.xrCrossTab1.TotalAreaStyleName = "crossTabTotalStyle1";
            // 
            // crossTabHeaderCell1
            // 
            this.crossTabHeaderCell1.ColumnIndex = 1;
            this.crossTabHeaderCell1.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabHeaderCell1.Name = "crossTabHeaderCell1";
            this.crossTabHeaderCell1.RowIndex = 0;
            this.crossTabHeaderCell1.Text = "Employee";
            // 
            // crossTabDataCell1
            // 
            this.crossTabDataCell1.ColumnIndex = 2;
            this.crossTabDataCell1.Font = new DevExpress.Drawing.DXFont("Arial", 8F);
            this.crossTabDataCell1.Name = "crossTabDataCell1";
            this.crossTabDataCell1.RowIndex = 1;
            this.crossTabDataCell1.TextFormatString = "{0:N2}";
            // 
            // crossTabHeaderCell2
            // 
            this.crossTabHeaderCell2.ColumnIndex = 2;
            this.crossTabHeaderCell2.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabHeaderCell2.Name = "crossTabHeaderCell2";
            this.crossTabHeaderCell2.RowIndex = 0;
            // 
            // crossTabHeaderCell3
            // 
            this.crossTabHeaderCell3.ColumnIndex = 1;
            this.crossTabHeaderCell3.Font = new DevExpress.Drawing.DXFont("Arial", 8F);
            this.crossTabHeaderCell3.Name = "crossTabHeaderCell3";
            this.crossTabHeaderCell3.RowIndex = 1;
            // 
            // crossTabHeaderCell4
            // 
            this.crossTabHeaderCell4.ColumnIndex = 0;
            this.crossTabHeaderCell4.ColumnSpan = 2;
            this.crossTabHeaderCell4.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabHeaderCell4.Name = "crossTabHeaderCell4";
            this.crossTabHeaderCell4.RowIndex = 3;
            this.crossTabHeaderCell4.Text = "Grand Total";
            // 
            // crossTabTotalCell1
            // 
            this.crossTabTotalCell1.ColumnIndex = 2;
            this.crossTabTotalCell1.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabTotalCell1.Name = "crossTabTotalCell1";
            this.crossTabTotalCell1.RowIndex = 3;
            this.crossTabTotalCell1.TextFormatString = "{0:N2}";
            // 
            // crossTabHeaderCell5
            // 
            this.crossTabHeaderCell5.ColumnIndex = 3;
            this.crossTabHeaderCell5.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabHeaderCell5.Name = "crossTabHeaderCell5";
            this.crossTabHeaderCell5.RowIndex = 0;
            this.crossTabHeaderCell5.Text = "Grand Total";
            // 
            // crossTabTotalCell2
            // 
            this.crossTabTotalCell2.ColumnIndex = 3;
            this.crossTabTotalCell2.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabTotalCell2.Name = "crossTabTotalCell2";
            this.crossTabTotalCell2.RowIndex = 1;
            this.crossTabTotalCell2.TextFormatString = "{0:N2}";
            // 
            // crossTabTotalCell3
            // 
            this.crossTabTotalCell3.ColumnIndex = 3;
            this.crossTabTotalCell3.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabTotalCell3.Name = "crossTabTotalCell3";
            this.crossTabTotalCell3.RowIndex = 3;
            this.crossTabTotalCell3.TextFormatString = "{0:N2}";
            // 
            // crossTabHeaderCell6
            // 
            this.crossTabHeaderCell6.ColumnIndex = 0;
            this.crossTabHeaderCell6.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabHeaderCell6.Name = "crossTabHeaderCell6";
            this.crossTabHeaderCell6.RowIndex = 0;
            this.crossTabHeaderCell6.Text = "Payroll Group";
            // 
            // crossTabHeaderCell7
            // 
            this.crossTabHeaderCell7.ColumnIndex = 0;
            this.crossTabHeaderCell7.Font = new DevExpress.Drawing.DXFont("Arial", 8F);
            this.crossTabHeaderCell7.Name = "crossTabHeaderCell7";
            this.crossTabHeaderCell7.RowIndex = 1;
            // 
            // crossTabHeaderCell8
            // 
            this.crossTabHeaderCell8.ColumnIndex = 0;
            this.crossTabHeaderCell8.ColumnSpan = 2;
            this.crossTabHeaderCell8.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabHeaderCell8.Name = "crossTabHeaderCell8";
            this.crossTabHeaderCell8.RowIndex = 2;
            this.crossTabHeaderCell8.TextFormatString = "Total {0}";
            // 
            // crossTabTotalCell4
            // 
            this.crossTabTotalCell4.ColumnIndex = 2;
            this.crossTabTotalCell4.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabTotalCell4.Name = "crossTabTotalCell4";
            this.crossTabTotalCell4.RowIndex = 2;
            this.crossTabTotalCell4.TextFormatString = "{0:N2}";
            // 
            // crossTabTotalCell5
            // 
            this.crossTabTotalCell5.ColumnIndex = 3;
            this.crossTabTotalCell5.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.crossTabTotalCell5.Name = "crossTabTotalCell5";
            this.crossTabTotalCell5.RowIndex = 2;
            this.crossTabTotalCell5.TextFormatString = "{0:N2}";
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "DefaultConnection";
            this.sqlDataSource1.Name = "sqlDataSource1";
            customSqlQuery1.Name = "Query";
            queryParameter1.Name = "ParamPayrollId";
            queryParameter1.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("?ParamPayrollId", typeof(int));
            queryParameter2.Name = "ParamDateStart";
            queryParameter2.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter2.Value = new DevExpress.DataAccess.Expression("?ParamDateStart", typeof(System.DateTime));
            queryParameter3.Name = "ParamDateEnd";
            queryParameter3.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter3.Value = new DevExpress.DataAccess.Expression("?ParamDateEnd", typeof(System.DateTime));
            queryParameter4.Name = "ParamPayrollNumber";
            queryParameter4.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter4.Value = new DevExpress.DataAccess.Expression("?ParamPayrollNumber", typeof(string));
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
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel2,
            this.xrLabel1,
            this.xrLabel3});
            this.TopMargin.HeightF = 86.45834F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Payroll Number: \' + ?ParamPayrollNumber")});
            this.xrLabel2.Font = new DevExpress.Drawing.DXFont("Arial", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(3.973643E-05F, 33F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(1479F, 23F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Payroll Number: ";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Font = new DevExpress.Drawing.DXFont("Arial", 12F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(1479F, 23F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "Payroll Deduction Cross Tab View";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "\'Date Range: \' + FormatString(\'{0:MM/dd/yyyy}\',?ParamDateStart) + \' - \' + FormatS" +
                    "tring(\'{0:MM/dd/yyyy}\',?ParamDateEnd)\n")});
            this.xrLabel3.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 56F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(1479F, 23F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // crossTabGeneralStyle1
            // 
            this.crossTabGeneralStyle1.BackColor = System.Drawing.Color.White;
            this.crossTabGeneralStyle1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.crossTabGeneralStyle1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.crossTabGeneralStyle1.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.crossTabGeneralStyle1.ForeColor = System.Drawing.Color.Black;
            this.crossTabGeneralStyle1.Name = "crossTabGeneralStyle1";
            this.crossTabGeneralStyle1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            // 
            // crossTabHeaderStyle1
            // 
            this.crossTabHeaderStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.crossTabHeaderStyle1.Name = "crossTabHeaderStyle1";
            this.crossTabHeaderStyle1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // crossTabDataStyle1
            // 
            this.crossTabDataStyle1.Name = "crossTabDataStyle1";
            this.crossTabDataStyle1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // crossTabTotalStyle1
            // 
            this.crossTabTotalStyle1.Name = "crossTabTotalStyle1";
            this.crossTabTotalStyle1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // ParamPayrollId
            // 
            this.ParamPayrollId.Name = "ParamPayrollId";
            this.ParamPayrollId.Type = typeof(int);
            this.ParamPayrollId.ValueInfo = "0";
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
            // ParamPayrollNumber
            // 
            this.ParamPayrollNumber.Name = "ParamPayrollNumber";
            // 
            // RepPayrollLoanDeductionCrossTab
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.Landscape = true;
            this.Margins = new DevExpress.Drawing.DXMargins(15F, 6F, 86.45834F, 100F);
            this.PageHeight = 927;
            this.PageWidth = 1500;
            this.PaperKind = System.Drawing.Printing.PaperKind.LegalExtra;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.ParamPayrollId,
            this.ParamDateStart,
            this.ParamDateEnd,
            this.ParamPayrollNumber});
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.crossTabGeneralStyle1,
            this.crossTabHeaderStyle1,
            this.crossTabDataStyle1,
            this.crossTabTotalStyle1});
            this.Version = "22.2";
            ((System.ComponentModel.ISupportInitialize)(this.xrCrossTab1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRCrossTab xrCrossTab1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabDataCell1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell2;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell3;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell4;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell1;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell5;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell2;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell3;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell6;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell7;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabHeaderCell8;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell4;
        private DevExpress.XtraReports.UI.CrossTab.XRCrossTabCell crossTabTotalCell5;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabGeneralStyle1;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabHeaderStyle1;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabDataStyle1;
        private DevExpress.XtraReports.UI.XRControlStyle crossTabTotalStyle1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.Parameters.Parameter ParamPayrollId;
        private DevExpress.XtraReports.Parameters.Parameter ParamDateStart;
        private DevExpress.XtraReports.Parameters.Parameter ParamDateEnd;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.Parameters.Parameter ParamPayrollNumber;
    }
}
