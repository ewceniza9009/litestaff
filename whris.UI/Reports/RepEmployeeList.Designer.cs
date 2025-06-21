namespace whris.UI.Reports
{
    partial class RepEmployeeList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepEmployeeList));
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel72 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel71 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel70 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel69 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel68 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel59 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel57 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel56 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel55 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel54 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel38 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel39 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel42 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.pageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel13 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.ParamPayrollGroupId = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1,
            this.xrLabel72,
            this.xrLabel71,
            this.xrLabel70,
            this.xrLabel69,
            this.xrLabel68,
            this.xrLabel59,
            this.xrLabel57,
            this.xrLabel56,
            this.xrLabel55,
            this.xrLabel54,
            this.xrLabel15,
            this.xrLabel38,
            this.xrLabel39,
            this.xrLabel42,
            this.xrLabel2,
            this.xrLabel4});
            this.TopMargin.HeightF = 163.3004F;
            this.TopMargin.Name = "TopMargin";
            // 
            // xrLabel1
            // 
            this.xrLabel1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PayrollGroup]")});
            this.xrLabel1.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 91.33334F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(1300F, 23F);
            this.xrLabel1.StylePriority.UseBorders = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "xrLabel7";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel72
            // 
            this.xrLabel72.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel72.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel72.LocationFloat = new DevExpress.Utils.PointFloat(611.4614F, 114.3339F);
            this.xrLabel72.Multiline = true;
            this.xrLabel72.Name = "xrLabel72";
            this.xrLabel72.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel72.SizeF = new System.Drawing.SizeF(77.0827F, 38.59051F);
            this.xrLabel72.StylePriority.UseBorders = false;
            this.xrLabel72.StylePriority.UseFont = false;
            this.xrLabel72.StylePriority.UsePadding = false;
            this.xrLabel72.StylePriority.UseTextAlignment = false;
            this.xrLabel72.Text = "Phone No.";
            this.xrLabel72.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel71
            // 
            this.xrLabel71.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel71.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel71.LocationFloat = new DevExpress.Utils.PointFloat(532.2953F, 114.3341F);
            this.xrLabel71.Multiline = true;
            this.xrLabel71.Name = "xrLabel71";
            this.xrLabel71.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel71.SizeF = new System.Drawing.SizeF(79.1662F, 38.5905F);
            this.xrLabel71.StylePriority.UseBorders = false;
            this.xrLabel71.StylePriority.UseFont = false;
            this.xrLabel71.StylePriority.UsePadding = false;
            this.xrLabel71.StylePriority.UseTextAlignment = false;
            this.xrLabel71.Text = "Zip Code";
            this.xrLabel71.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel70
            // 
            this.xrLabel70.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel70.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel70.LocationFloat = new DevExpress.Utils.PointFloat(314.5877F, 114.3333F);
            this.xrLabel70.Multiline = true;
            this.xrLabel70.Name = "xrLabel70";
            this.xrLabel70.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel70.SizeF = new System.Drawing.SizeF(217.7077F, 38.59053F);
            this.xrLabel70.StylePriority.UseBorders = false;
            this.xrLabel70.StylePriority.UseFont = false;
            this.xrLabel70.StylePriority.UsePadding = false;
            this.xrLabel70.StylePriority.UseTextAlignment = false;
            this.xrLabel70.Text = "Address";
            this.xrLabel70.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel69
            // 
            this.xrLabel69.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel69.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel69.LocationFloat = new DevExpress.Utils.PointFloat(234.3801F, 114.3333F);
            this.xrLabel69.Multiline = true;
            this.xrLabel69.Name = "xrLabel69";
            this.xrLabel69.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel69.SizeF = new System.Drawing.SizeF(80.20766F, 38.59053F);
            this.xrLabel69.StylePriority.UseBorders = false;
            this.xrLabel69.StylePriority.UseFont = false;
            this.xrLabel69.StylePriority.UsePadding = false;
            this.xrLabel69.StylePriority.UseTextAlignment = false;
            this.xrLabel69.Text = "Position";
            this.xrLabel69.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel68
            // 
            this.xrLabel68.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel68.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel68.LocationFloat = new DevExpress.Utils.PointFloat(185.4174F, 114.3333F);
            this.xrLabel68.Multiline = true;
            this.xrLabel68.Name = "xrLabel68";
            this.xrLabel68.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel68.SizeF = new System.Drawing.SizeF(48.96271F, 38.59052F);
            this.xrLabel68.StylePriority.UseBorders = false;
            this.xrLabel68.StylePriority.UseFont = false;
            this.xrLabel68.StylePriority.UsePadding = false;
            this.xrLabel68.StylePriority.UseTextAlignment = false;
            this.xrLabel68.Text = "Id Number";
            this.xrLabel68.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel59
            // 
            this.xrLabel59.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel59.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel59.LocationFloat = new DevExpress.Utils.PointFloat(1218.75F, 114.3337F);
            this.xrLabel59.Multiline = true;
            this.xrLabel59.Name = "xrLabel59";
            this.xrLabel59.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel59.SizeF = new System.Drawing.SizeF(79.16675F, 38.59071F);
            this.xrLabel59.StylePriority.UseBorders = false;
            this.xrLabel59.StylePriority.UseFont = false;
            this.xrLabel59.StylePriority.UsePadding = false;
            this.xrLabel59.StylePriority.UseTextAlignment = false;
            this.xrLabel59.Text = "Daily Rate";
            this.xrLabel59.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel57
            // 
            this.xrLabel57.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel57.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel57.LocationFloat = new DevExpress.Utils.PointFloat(1138.542F, 114.3339F);
            this.xrLabel57.Multiline = true;
            this.xrLabel57.Name = "xrLabel57";
            this.xrLabel57.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel57.SizeF = new System.Drawing.SizeF(80.20837F, 38.5907F);
            this.xrLabel57.StylePriority.UseBorders = false;
            this.xrLabel57.StylePriority.UseFont = false;
            this.xrLabel57.StylePriority.UsePadding = false;
            this.xrLabel57.StylePriority.UseTextAlignment = false;
            this.xrLabel57.Text = "Monthly Rate";
            this.xrLabel57.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel56
            // 
            this.xrLabel56.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel56.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel56.LocationFloat = new DevExpress.Utils.PointFloat(1064.583F, 114.3337F);
            this.xrLabel56.Multiline = true;
            this.xrLabel56.Name = "xrLabel56";
            this.xrLabel56.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel56.SizeF = new System.Drawing.SizeF(73.95837F, 38.59071F);
            this.xrLabel56.StylePriority.UseBorders = false;
            this.xrLabel56.StylePriority.UseFont = false;
            this.xrLabel56.StylePriority.UsePadding = false;
            this.xrLabel56.StylePriority.UseTextAlignment = false;
            this.xrLabel56.Text = "Civil Status";
            this.xrLabel56.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel55
            // 
            this.xrLabel55.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel55.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel55.LocationFloat = new DevExpress.Utils.PointFloat(1008.333F, 114.3334F);
            this.xrLabel55.Multiline = true;
            this.xrLabel55.Name = "xrLabel55";
            this.xrLabel55.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel55.SizeF = new System.Drawing.SizeF(56.24988F, 38.5907F);
            this.xrLabel55.StylePriority.UseBorders = false;
            this.xrLabel55.StylePriority.UseFont = false;
            this.xrLabel55.StylePriority.UsePadding = false;
            this.xrLabel55.StylePriority.UseTextAlignment = false;
            this.xrLabel55.Text = "Sex";
            this.xrLabel55.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel54
            // 
            this.xrLabel54.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel54.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel54.LocationFloat = new DevExpress.Utils.PointFloat(950.0002F, 114.3334F);
            this.xrLabel54.Multiline = true;
            this.xrLabel54.Name = "xrLabel54";
            this.xrLabel54.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel54.SizeF = new System.Drawing.SizeF(58.33319F, 38.5907F);
            this.xrLabel54.StylePriority.UseBorders = false;
            this.xrLabel54.StylePriority.UseFont = false;
            this.xrLabel54.StylePriority.UsePadding = false;
            this.xrLabel54.StylePriority.UseTextAlignment = false;
            this.xrLabel54.Text = "Date Resigned";
            this.xrLabel54.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel15
            // 
            this.xrLabel15.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel15.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(0F, 114.3334F);
            this.xrLabel15.Multiline = true;
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(185.4174F, 38.59068F);
            this.xrLabel15.StylePriority.UseBorders = false;
            this.xrLabel15.StylePriority.UseFont = false;
            this.xrLabel15.StylePriority.UsePadding = false;
            this.xrLabel15.StylePriority.UseTextAlignment = false;
            this.xrLabel15.Text = "Employee";
            this.xrLabel15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel38
            // 
            this.xrLabel38.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel38.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel38.LocationFloat = new DevExpress.Utils.PointFloat(890.6258F, 114.3334F);
            this.xrLabel38.Multiline = true;
            this.xrLabel38.Name = "xrLabel38";
            this.xrLabel38.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel38.SizeF = new System.Drawing.SizeF(59.37445F, 38.5907F);
            this.xrLabel38.StylePriority.UseBorders = false;
            this.xrLabel38.StylePriority.UseFont = false;
            this.xrLabel38.StylePriority.UsePadding = false;
            this.xrLabel38.StylePriority.UseTextAlignment = false;
            this.xrLabel38.Text = "Date Hired";
            this.xrLabel38.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel39
            // 
            this.xrLabel39.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel39.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel39.LocationFloat = new DevExpress.Utils.PointFloat(688.5441F, 114.3333F);
            this.xrLabel39.Multiline = true;
            this.xrLabel39.Name = "xrLabel39";
            this.xrLabel39.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel39.SizeF = new System.Drawing.SizeF(83.33276F, 38.5907F);
            this.xrLabel39.StylePriority.UseBorders = false;
            this.xrLabel39.StylePriority.UseFont = false;
            this.xrLabel39.StylePriority.UsePadding = false;
            this.xrLabel39.StylePriority.UseTextAlignment = false;
            this.xrLabel39.Text = "Cell No.";
            this.xrLabel39.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel42
            // 
            this.xrLabel42.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel42.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel42.LocationFloat = new DevExpress.Utils.PointFloat(837.5013F, 114.3333F);
            this.xrLabel42.Multiline = true;
            this.xrLabel42.Name = "xrLabel42";
            this.xrLabel42.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel42.SizeF = new System.Drawing.SizeF(53.12451F, 38.59071F);
            this.xrLabel42.StylePriority.UseBorders = false;
            this.xrLabel42.StylePriority.UseFont = false;
            this.xrLabel42.StylePriority.UsePadding = false;
            this.xrLabel42.StylePriority.UseTextAlignment = false;
            this.xrLabel42.Text = "Age";
            this.xrLabel42.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel2
            // 
            this.xrLabel2.Font = new DevExpress.Drawing.DXFont("Arial", 12F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 68.33334F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(1300F, 23F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Employee List";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel4
            // 
            this.xrLabel4.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrLabel4.Font = new DevExpress.Drawing.DXFont("Arial", 8F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(771.8769F, 114.3341F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 5, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(65.62445F, 38.5907F);
            this.xrLabel4.StylePriority.UseBorders = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UsePadding = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.Text = "Date of Birth";
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageInfo1,
            this.pageInfo2});
            this.BottomMargin.Name = "BottomMargin";
            // 
            // pageInfo1
            // 
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 77F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.pageInfo1.SizeF = new System.Drawing.SizeF(325F, 23F);
            // 
            // pageInfo2
            // 
            this.pageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(975F, 76.99998F);
            this.pageInfo2.Name = "pageInfo2";
            this.pageInfo2.SizeF = new System.Drawing.SizeF(325F, 23F);
            this.pageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.pageInfo2.TextFormatString = "Page {0} of {1}";
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel19,
            this.xrLabel18,
            this.xrLabel17,
            this.xrLabel16,
            this.xrLabel14,
            this.xrLabel13,
            this.xrLabel12,
            this.xrLabel11,
            this.xrLabel10,
            this.xrLabel9,
            this.xrLabel8,
            this.xrLabel6,
            this.xrLabel5,
            this.xrLabel3,
            this.xrLabel7});
            this.Detail.HeightF = 23.00006F;
            this.Detail.Name = "Detail";
            this.Detail.SortFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("FullName", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            // 
            // xrLabel19
            // 
            this.xrLabel19.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel19.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Age]")});
            this.xrLabel19.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(837.5013F, 6.357829E-05F);
            this.xrLabel19.Multiline = true;
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(53.12445F, 23F);
            this.xrLabel19.StylePriority.UseBorders = false;
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.Text = "xrLabel7";
            this.xrLabel19.TextFormatString = "{0:N0}";
            // 
            // xrLabel18
            // 
            this.xrLabel18.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel18.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DailyRate]")});
            this.xrLabel18.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(1218.75F, 0F);
            this.xrLabel18.Multiline = true;
            this.xrLabel18.Name = "xrLabel18";
            this.xrLabel18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel18.SizeF = new System.Drawing.SizeF(79.16687F, 23F);
            this.xrLabel18.StylePriority.UseBorders = false;
            this.xrLabel18.StylePriority.UseFont = false;
            this.xrLabel18.StylePriority.UseTextAlignment = false;
            this.xrLabel18.Text = "xrLabel7";
            this.xrLabel18.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel18.TextFormatString = "{0:N2}";
            // 
            // xrLabel17
            // 
            this.xrLabel17.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel17.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[MonthlyRate]")});
            this.xrLabel17.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(1138.542F, 0F);
            this.xrLabel17.Multiline = true;
            this.xrLabel17.Name = "xrLabel17";
            this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel17.SizeF = new System.Drawing.SizeF(80.2085F, 23F);
            this.xrLabel17.StylePriority.UseBorders = false;
            this.xrLabel17.StylePriority.UseFont = false;
            this.xrLabel17.StylePriority.UseTextAlignment = false;
            this.xrLabel17.Text = "xrLabel7";
            this.xrLabel17.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrLabel17.TextFormatString = "{0:N2}";
            // 
            // xrLabel16
            // 
            this.xrLabel16.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel16.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CivilStatus]")});
            this.xrLabel16.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(1064.583F, 0F);
            this.xrLabel16.Multiline = true;
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel16.SizeF = new System.Drawing.SizeF(73.95801F, 23F);
            this.xrLabel16.StylePriority.UseBorders = false;
            this.xrLabel16.StylePriority.UseFont = false;
            this.xrLabel16.Text = "xrLabel7";
            // 
            // xrLabel14
            // 
            this.xrLabel14.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel14.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Sex]")});
            this.xrLabel14.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(1008.333F, 6.357829E-05F);
            this.xrLabel14.Multiline = true;
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(56.25006F, 23F);
            this.xrLabel14.StylePriority.UseBorders = false;
            this.xrLabel14.StylePriority.UseFont = false;
            this.xrLabel14.Text = "xrLabel7";
            // 
            // xrLabel13
            // 
            this.xrLabel13.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel13.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DateResigned]")});
            this.xrLabel13.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel13.LocationFloat = new DevExpress.Utils.PointFloat(950.0002F, 0F);
            this.xrLabel13.Multiline = true;
            this.xrLabel13.Name = "xrLabel13";
            this.xrLabel13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel13.SizeF = new System.Drawing.SizeF(58.33307F, 23F);
            this.xrLabel13.StylePriority.UseBorders = false;
            this.xrLabel13.StylePriority.UseFont = false;
            this.xrLabel13.Text = "xrLabel7";
            this.xrLabel13.TextFormatString = "{0:d}";
            // 
            // xrLabel12
            // 
            this.xrLabel12.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel12.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DateHired]")});
            this.xrLabel12.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(890.6258F, 0F);
            this.xrLabel12.Multiline = true;
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(59.37445F, 23F);
            this.xrLabel12.StylePriority.UseBorders = false;
            this.xrLabel12.StylePriority.UseFont = false;
            this.xrLabel12.Text = "xrLabel7";
            this.xrLabel12.TextFormatString = "{0:d}";
            // 
            // xrLabel11
            // 
            this.xrLabel11.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DateOfBirth]")});
            this.xrLabel11.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(771.8769F, 6.357829E-05F);
            this.xrLabel11.Multiline = true;
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(65.62445F, 23F);
            this.xrLabel11.StylePriority.UseBorders = false;
            this.xrLabel11.StylePriority.UseFont = false;
            this.xrLabel11.Text = "xrLabel7";
            this.xrLabel11.TextFormatString = "{0:d}";
            // 
            // xrLabel10
            // 
            this.xrLabel10.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel10.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CellphoneNumber]")});
            this.xrLabel10.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(688.544F, 6.357829E-05F);
            this.xrLabel10.Multiline = true;
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(83.33282F, 23F);
            this.xrLabel10.StylePriority.UseBorders = false;
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.Text = "xrLabel7";
            // 
            // xrLabel9
            // 
            this.xrLabel9.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PhoneNumBEr]")});
            this.xrLabel9.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(611.4614F, 0F);
            this.xrLabel9.Multiline = true;
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(77.08258F, 23F);
            this.xrLabel9.StylePriority.UseBorders = false;
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.Text = "xrLabel7";
            // 
            // xrLabel8
            // 
            this.xrLabel8.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Zipcode]")});
            this.xrLabel8.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(532.2955F, 6.357829E-05F);
            this.xrLabel8.Multiline = true;
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(79.16602F, 23F);
            this.xrLabel8.StylePriority.UseBorders = false;
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.Text = "xrLabel7";
            // 
            // xrLabel6
            // 
            this.xrLabel6.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Address]")});
            this.xrLabel6.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(314.5877F, 6.357829E-05F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(217.7078F, 23F);
            this.xrLabel6.StylePriority.UseBorders = false;
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.Text = "xrLabel7";
            // 
            // xrLabel5
            // 
            this.xrLabel5.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Position]")});
            this.xrLabel5.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(234.3802F, 0F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(80.20755F, 23F);
            this.xrLabel5.StylePriority.UseBorders = false;
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.Text = "xrLabel7";
            // 
            // xrLabel3
            // 
            this.xrLabel3.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[IdNumber]")});
            this.xrLabel3.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(185.4174F, 6.357829E-05F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(48.96272F, 23F);
            this.xrLabel3.StylePriority.UseBorders = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "xrLabel7";
            // 
            // xrLabel7
            // 
            this.xrLabel7.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[FullName]")});
            this.xrLabel7.Font = new DevExpress.Drawing.DXFont("Arial", 7F);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel7.Multiline = true;
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(185.4174F, 23F);
            this.xrLabel7.StylePriority.UseBorders = false;
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.Text = "xrLabel7";
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "DefaultConnection";
            this.sqlDataSource1.Name = "sqlDataSource1";
            customSqlQuery1.Name = "Query";
            queryParameter1.Name = "ParamPayrollGroupId";
            queryParameter1.Type = typeof(global::DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("?ParamPayrollGroupId", typeof(int));
            customSqlQuery1.Parameters.AddRange(new DevExpress.DataAccess.Sql.QueryParameter[] {
            queryParameter1});
            customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            customSqlQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // PageFooter
            // 
            this.PageFooter.HeightF = 0F;
            this.PageFooter.Name = "PageFooter";
            // 
            // ParamPayrollGroupId
            // 
            this.ParamPayrollGroupId.Name = "ParamPayrollGroupId";
            this.ParamPayrollGroupId.Type = typeof(int);
            this.ParamPayrollGroupId.ValueInfo = "0";
            // 
            // RepEmployeeList
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.Detail,
            this.PageFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "Query";
            this.DataSource = this.sqlDataSource1;
            this.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.Landscape = true;
            this.Margins = new DevExpress.Drawing.DXMargins(100F, 100F, 163.3004F, 100F);
            this.PageHeight = 927;
            this.PageWidth = 1500;
            this.PaperKind = System.Drawing.Printing.PaperKind.LegalExtra;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.ParamPayrollGroupId});
            this.Version = "22.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel15;
        private DevExpress.XtraReports.UI.XRLabel xrLabel38;
        private DevExpress.XtraReports.UI.XRLabel xrLabel39;
        private DevExpress.XtraReports.UI.XRLabel xrLabel42;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel59;
        private DevExpress.XtraReports.UI.XRLabel xrLabel57;
        private DevExpress.XtraReports.UI.XRLabel xrLabel56;
        private DevExpress.XtraReports.UI.XRLabel xrLabel55;
        private DevExpress.XtraReports.UI.XRLabel xrLabel54;
        private DevExpress.XtraReports.UI.XRLabel xrLabel72;
        private DevExpress.XtraReports.UI.XRLabel xrLabel71;
        private DevExpress.XtraReports.UI.XRLabel xrLabel70;
        private DevExpress.XtraReports.UI.XRLabel xrLabel69;
        private DevExpress.XtraReports.UI.XRLabel xrLabel68;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
        private DevExpress.XtraReports.UI.XRLabel xrLabel18;
        private DevExpress.XtraReports.UI.XRLabel xrLabel17;
        private DevExpress.XtraReports.UI.XRLabel xrLabel16;
        private DevExpress.XtraReports.UI.XRLabel xrLabel14;
        private DevExpress.XtraReports.UI.XRLabel xrLabel13;
        private DevExpress.XtraReports.UI.XRLabel xrLabel12;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.Parameters.Parameter ParamPayrollGroupId;
    }
}
