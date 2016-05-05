namespace Phoenix.Test.UI
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox_ClientId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_TestPlan = new System.Windows.Forms.Button();
            this.textBox_ClientKey = new System.Windows.Forms.TextBox();
            this.textBox_TenantId = new System.Windows.Forms.TextBox();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_ServerName = new System.Windows.Forms.TextBox();
            this.tabControlTesting = new System.Windows.Forms.TabControl();
            this.postInstallPage = new System.Windows.Forms.TabPage();
            this.allTestsPic = new System.Windows.Forms.PictureBox();
            this.lblAllTests = new System.Windows.Forms.Label();
            this.infraCheckPic = new System.Windows.Forms.PictureBox();
            this.lblCheckInfra = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.resourceProviderPic = new System.Windows.Forms.PictureBox();
            this.lblCheckResourceProvider = new System.Windows.Forms.Label();
            this.webappsPic = new System.Windows.Forms.PictureBox();
            this.servicesPic = new System.Windows.Forms.PictureBox();
            this.databasesPic = new System.Windows.Forms.PictureBox();
            this.lblCheckWebApps = new System.Windows.Forms.Label();
            this.lblCheckServices = new System.Windows.Forms.Label();
            this.lblCheckDatabases = new System.Windows.Forms.Label();
            this.lblPostInstallSteps = new System.Windows.Forms.Label();
            this.cmpwapProgressBar = new System.Windows.Forms.ProgressBar();
            this.adminUIPage = new System.Windows.Forms.TabPage();
            this.adminPortalLabel = new System.Windows.Forms.Label();
            this.regions = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tenantUserPsw = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tenantUserAccount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_AdminPortalServer = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tenantUIPage = new System.Windows.Forms.TabPage();
            this.tenantPortalLabel = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_TenantPortalServer = new System.Windows.Forms.TextBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabControlTesting.SuspendLayout();
            this.postInstallPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.allTestsPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infraCheckPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resourceProviderPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webappsPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.servicesPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.databasesPic)).BeginInit();
            this.adminUIPage.SuspendLayout();
            this.tenantUIPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(177)))), ((int)(((byte)(209)))));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(137, 471);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(307, 58);
            this.button1.TabIndex = 12;
            this.button1.Text = "START ADMIN PORTAL UI TEST";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(177)))), ((int)(((byte)(209)))));
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(154, 129);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(290, 59);
            this.button2.TabIndex = 16;
            this.button2.Text = "START TENANT PORTAL UI TEST";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox_ClientId
            // 
            this.textBox_ClientId.Location = new System.Drawing.Point(190, 104);
            this.textBox_ClientId.Name = "textBox_ClientId";
            this.textBox_ClientId.Size = new System.Drawing.Size(306, 24);
            this.textBox_ClientId.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(23, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "TENANT ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(23, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "CLIENT ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(23, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "CLIENT KEY";
            // 
            // btn_TestPlan
            // 
            this.btn_TestPlan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(113)))), ((int)(((byte)(177)))), ((int)(((byte)(209)))));
            this.btn_TestPlan.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.btn_TestPlan.ForeColor = System.Drawing.Color.White;
            this.btn_TestPlan.Location = new System.Drawing.Point(155, 46);
            this.btn_TestPlan.Name = "btn_TestPlan";
            this.btn_TestPlan.Size = new System.Drawing.Size(306, 58);
            this.btn_TestPlan.TabIndex = 4;
            this.btn_TestPlan.Text = "START CMPWAP INSTALLATION TEST";
            this.btn_TestPlan.UseVisualStyleBackColor = false;
            this.btn_TestPlan.Click += new System.EventHandler(this.btn_TestPlan_Click);
            // 
            // textBox_ClientKey
            // 
            this.textBox_ClientKey.Location = new System.Drawing.Point(190, 142);
            this.textBox_ClientKey.Name = "textBox_ClientKey";
            this.textBox_ClientKey.Size = new System.Drawing.Size(306, 24);
            this.textBox_ClientKey.TabIndex = 8;
            // 
            // textBox_TenantId
            // 
            this.textBox_TenantId.Location = new System.Drawing.Point(190, 182);
            this.textBox_TenantId.Name = "textBox_TenantId";
            this.textBox_TenantId.Size = new System.Drawing.Size(306, 24);
            this.textBox_TenantId.TabIndex = 9;
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.Location = new System.Drawing.Point(190, 32);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.Size = new System.Drawing.Size(306, 24);
            this.textBox_UserName.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(23, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 15);
            this.label6.TabIndex = 1;
            this.label6.Text = "USER ACCOUNT";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(23, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "PASSWORD";
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(190, 66);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.PasswordChar = '*';
            this.textBox_Password.Size = new System.Drawing.Size(306, 24);
            this.textBox_Password.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(28, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(109, 15);
            this.label8.TabIndex = 16;
            this.label8.Text = "CMP SERVER NAME";
            // 
            // textBox_ServerName
            // 
            this.textBox_ServerName.Location = new System.Drawing.Point(155, 16);
            this.textBox_ServerName.Name = "textBox_ServerName";
            this.textBox_ServerName.Size = new System.Drawing.Size(306, 24);
            this.textBox_ServerName.TabIndex = 3;
            // 
            // tabControlTesting
            // 
            this.tabControlTesting.Controls.Add(this.postInstallPage);
            this.tabControlTesting.Controls.Add(this.adminUIPage);
            this.tabControlTesting.Controls.Add(this.tenantUIPage);
            this.tabControlTesting.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControlTesting.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.tabControlTesting.ItemSize = new System.Drawing.Size(201, 49);
            this.tabControlTesting.Location = new System.Drawing.Point(0, 0);
            this.tabControlTesting.Multiline = true;
            this.tabControlTesting.Name = "tabControlTesting";
            this.tabControlTesting.SelectedIndex = 0;
            this.tabControlTesting.Size = new System.Drawing.Size(608, 608);
            this.tabControlTesting.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlTesting.TabIndex = 0;
            // 
            // postInstallPage
            // 
            this.postInstallPage.BackColor = System.Drawing.Color.White;
            this.postInstallPage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.postInstallPage.Controls.Add(this.allTestsPic);
            this.postInstallPage.Controls.Add(this.lblAllTests);
            this.postInstallPage.Controls.Add(this.infraCheckPic);
            this.postInstallPage.Controls.Add(this.lblCheckInfra);
            this.postInstallPage.Controls.Add(this.label12);
            this.postInstallPage.Controls.Add(this.resourceProviderPic);
            this.postInstallPage.Controls.Add(this.lblCheckResourceProvider);
            this.postInstallPage.Controls.Add(this.webappsPic);
            this.postInstallPage.Controls.Add(this.servicesPic);
            this.postInstallPage.Controls.Add(this.databasesPic);
            this.postInstallPage.Controls.Add(this.lblCheckWebApps);
            this.postInstallPage.Controls.Add(this.lblCheckServices);
            this.postInstallPage.Controls.Add(this.lblCheckDatabases);
            this.postInstallPage.Controls.Add(this.lblPostInstallSteps);
            this.postInstallPage.Controls.Add(this.cmpwapProgressBar);
            this.postInstallPage.Controls.Add(this.label8);
            this.postInstallPage.Controls.Add(this.textBox_ServerName);
            this.postInstallPage.Controls.Add(this.btn_TestPlan);
            this.postInstallPage.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.postInstallPage.ImageIndex = 0;
            this.postInstallPage.Location = new System.Drawing.Point(4, 53);
            this.postInstallPage.Name = "postInstallPage";
            this.postInstallPage.Padding = new System.Windows.Forms.Padding(3);
            this.postInstallPage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.postInstallPage.Size = new System.Drawing.Size(600, 551);
            this.postInstallPage.TabIndex = 0;
            this.postInstallPage.Text = "INSTALLATION TEST";
            // 
            // allTestsPic
            // 
            this.allTestsPic.BackColor = System.Drawing.Color.Transparent;
            this.allTestsPic.Location = new System.Drawing.Point(161, 289);
            this.allTestsPic.Name = "allTestsPic";
            this.allTestsPic.Size = new System.Drawing.Size(16, 16);
            this.allTestsPic.TabIndex = 33;
            this.allTestsPic.TabStop = false;
            // 
            // lblAllTests
            // 
            this.lblAllTests.BackColor = System.Drawing.Color.White;
            this.lblAllTests.Location = new System.Drawing.Point(181, 289);
            this.lblAllTests.Name = "lblAllTests";
            this.lblAllTests.Size = new System.Drawing.Size(285, 17);
            this.lblAllTests.TabIndex = 32;
            this.lblAllTests.Text = "INSTALLATION TEST IN PROGRESS";
            this.lblAllTests.Visible = false;
            // 
            // infraCheckPic
            // 
            this.infraCheckPic.BackColor = System.Drawing.Color.Transparent;
            this.infraCheckPic.Location = new System.Drawing.Point(161, 150);
            this.infraCheckPic.Name = "infraCheckPic";
            this.infraCheckPic.Size = new System.Drawing.Size(16, 16);
            this.infraCheckPic.TabIndex = 31;
            this.infraCheckPic.TabStop = false;
            // 
            // lblCheckInfra
            // 
            this.lblCheckInfra.Location = new System.Drawing.Point(181, 150);
            this.lblCheckInfra.Name = "lblCheckInfra";
            this.lblCheckInfra.Size = new System.Drawing.Size(411, 17);
            this.lblCheckInfra.TabIndex = 30;
            this.lblCheckInfra.Text = "GATHER INFRASTRUCTURE INFORMATION";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(229, 461);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 23);
            this.label12.TabIndex = 34;
            // 
            // resourceProviderPic
            // 
            this.resourceProviderPic.BackColor = System.Drawing.Color.Transparent;
            this.resourceProviderPic.Location = new System.Drawing.Point(161, 179);
            this.resourceProviderPic.Name = "resourceProviderPic";
            this.resourceProviderPic.Size = new System.Drawing.Size(16, 16);
            this.resourceProviderPic.TabIndex = 29;
            this.resourceProviderPic.TabStop = false;
            // 
            // lblCheckResourceProvider
            // 
            this.lblCheckResourceProvider.Location = new System.Drawing.Point(181, 177);
            this.lblCheckResourceProvider.Name = "lblCheckResourceProvider";
            this.lblCheckResourceProvider.Size = new System.Drawing.Size(286, 17);
            this.lblCheckResourceProvider.TabIndex = 28;
            this.lblCheckResourceProvider.Text = "CHECK RESOURCE PROVIDER";
            // 
            // webappsPic
            // 
            this.webappsPic.BackColor = System.Drawing.Color.Transparent;
            this.webappsPic.Location = new System.Drawing.Point(161, 263);
            this.webappsPic.Name = "webappsPic";
            this.webappsPic.Size = new System.Drawing.Size(16, 16);
            this.webappsPic.TabIndex = 25;
            this.webappsPic.TabStop = false;
            // 
            // servicesPic
            // 
            this.servicesPic.BackColor = System.Drawing.Color.Transparent;
            this.servicesPic.Location = new System.Drawing.Point(161, 236);
            this.servicesPic.Name = "servicesPic";
            this.servicesPic.Size = new System.Drawing.Size(16, 16);
            this.servicesPic.TabIndex = 24;
            this.servicesPic.TabStop = false;
            // 
            // databasesPic
            // 
            this.databasesPic.BackColor = System.Drawing.Color.Transparent;
            this.databasesPic.Location = new System.Drawing.Point(161, 208);
            this.databasesPic.Name = "databasesPic";
            this.databasesPic.Size = new System.Drawing.Size(16, 16);
            this.databasesPic.TabIndex = 23;
            this.databasesPic.TabStop = false;
            // 
            // lblCheckWebApps
            // 
            this.lblCheckWebApps.AutoSize = true;
            this.lblCheckWebApps.BackColor = System.Drawing.Color.White;
            this.lblCheckWebApps.Location = new System.Drawing.Point(181, 260);
            this.lblCheckWebApps.Name = "lblCheckWebApps";
            this.lblCheckWebApps.Size = new System.Drawing.Size(109, 17);
            this.lblCheckWebApps.TabIndex = 22;
            this.lblCheckWebApps.Text = "CHECK WEB APPS";
            // 
            // lblCheckServices
            // 
            this.lblCheckServices.AutoSize = true;
            this.lblCheckServices.Location = new System.Drawing.Point(181, 234);
            this.lblCheckServices.Name = "lblCheckServices";
            this.lblCheckServices.Size = new System.Drawing.Size(126, 17);
            this.lblCheckServices.TabIndex = 21;
            this.lblCheckServices.Text = "CHECK CMP SERVICE";
            // 
            // lblCheckDatabases
            // 
            this.lblCheckDatabases.AutoSize = true;
            this.lblCheckDatabases.Location = new System.Drawing.Point(181, 206);
            this.lblCheckDatabases.Name = "lblCheckDatabases";
            this.lblCheckDatabases.Size = new System.Drawing.Size(116, 17);
            this.lblCheckDatabases.TabIndex = 20;
            this.lblCheckDatabases.Text = "CHECK DATABASES";
            // 
            // lblPostInstallSteps
            // 
            this.lblPostInstallSteps.AutoSize = true;
            this.lblPostInstallSteps.BackColor = System.Drawing.Color.Transparent;
            this.lblPostInstallSteps.ForeColor = System.Drawing.Color.Black;
            this.lblPostInstallSteps.Location = new System.Drawing.Point(152, 123);
            this.lblPostInstallSteps.Name = "lblPostInstallSteps";
            this.lblPostInstallSteps.Size = new System.Drawing.Size(163, 17);
            this.lblPostInstallSteps.TabIndex = 19;
            this.lblPostInstallSteps.Text = "INSTALLATION TEST STEPS:";
            // 
            // cmpwapProgressBar
            // 
            this.cmpwapProgressBar.ForeColor = System.Drawing.Color.Teal;
            this.cmpwapProgressBar.Location = new System.Drawing.Point(155, 327);
            this.cmpwapProgressBar.Name = "cmpwapProgressBar";
            this.cmpwapProgressBar.Size = new System.Drawing.Size(306, 73);
            this.cmpwapProgressBar.Step = 5;
            this.cmpwapProgressBar.TabIndex = 18;
            this.cmpwapProgressBar.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // adminUIPage
            // 
            this.adminUIPage.Controls.Add(this.adminPortalLabel);
            this.adminUIPage.Controls.Add(this.regions);
            this.adminUIPage.Controls.Add(this.label9);
            this.adminUIPage.Controls.Add(this.label15);
            this.adminUIPage.Controls.Add(this.tenantUserPsw);
            this.adminUIPage.Controls.Add(this.label14);
            this.adminUIPage.Controls.Add(this.tenantUserAccount);
            this.adminUIPage.Controls.Add(this.label5);
            this.adminUIPage.Controls.Add(this.textBox_AdminPortalServer);
            this.adminUIPage.Controls.Add(this.label4);
            this.adminUIPage.Controls.Add(this.textBox1);
            this.adminUIPage.Controls.Add(this.label6);
            this.adminUIPage.Controls.Add(this.textBox_Password);
            this.adminUIPage.Controls.Add(this.textBox_UserName);
            this.adminUIPage.Controls.Add(this.button1);
            this.adminUIPage.Controls.Add(this.textBox_TenantId);
            this.adminUIPage.Controls.Add(this.label7);
            this.adminUIPage.Controls.Add(this.textBox_ClientKey);
            this.adminUIPage.Controls.Add(this.label1);
            this.adminUIPage.Controls.Add(this.label2);
            this.adminUIPage.Controls.Add(this.textBox_ClientId);
            this.adminUIPage.Controls.Add(this.label3);
            this.adminUIPage.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.adminUIPage.Location = new System.Drawing.Point(4, 53);
            this.adminUIPage.Name = "adminUIPage";
            this.adminUIPage.Padding = new System.Windows.Forms.Padding(3);
            this.adminUIPage.Size = new System.Drawing.Size(600, 551);
            this.adminUIPage.TabIndex = 1;
            this.adminUIPage.Text = "ADMIN PORTAL UI TEST";
            this.adminUIPage.UseVisualStyleBackColor = true;
            // 
            // adminPortalLabel
            // 
            this.adminPortalLabel.AutoSize = true;
            this.adminPortalLabel.Location = new System.Drawing.Point(27, 433);
            this.adminPortalLabel.Name = "adminPortalLabel";
            this.adminPortalLabel.Size = new System.Drawing.Size(151, 17);
            this.adminPortalLabel.TabIndex = 27;
            this.adminPortalLabel.Text = "ADMIN PORTAL UI TESTS";
            this.adminPortalLabel.Visible = false;
            this.adminPortalLabel.Click += new System.EventHandler(this.adminPortalLabel_Click);
            // 
            // regions
            // 
            this.regions.FormattingEnabled = true;
            this.regions.Items.AddRange(new object[] {
            "Australia East",
            "Australia Southeast",
            "Central US",
            "East US",
            "East US 2",
            "North Central US",
            "South Central US",
            "West US"});
            this.regions.Location = new System.Drawing.Point(190, 386);
            this.regions.Name = "regions";
            this.regions.Size = new System.Drawing.Size(306, 23);
            this.regions.TabIndex = 26;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(23, 388);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(131, 15);
            this.label9.TabIndex = 24;
            this.label9.Text = "SUBSCRIPTION REGION";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(23, 351);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(142, 15);
            this.label15.TabIndex = 23;
            this.label15.Text = "TENANT USER PASSWORD";
            // 
            // tenantUserPsw
            // 
            this.tenantUserPsw.Location = new System.Drawing.Point(190, 347);
            this.tenantUserPsw.Name = "tenantUserPsw";
            this.tenantUserPsw.PasswordChar = '*';
            this.tenantUserPsw.Size = new System.Drawing.Size(306, 24);
            this.tenantUserPsw.TabIndex = 22;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(23, 307);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(133, 15);
            this.label14.TabIndex = 21;
            this.label14.Text = "TENANT USER ACCOUNT";
            // 
            // tenantUserAccount
            // 
            this.tenantUserAccount.Location = new System.Drawing.Point(190, 303);
            this.tenantUserAccount.Name = "tenantUserAccount";
            this.tenantUserAccount.Size = new System.Drawing.Size(306, 24);
            this.tenantUserAccount.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(23, 263);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 15);
            this.label5.TabIndex = 19;
            this.label5.Text = "ADMIN PORTAL SERVER";
            // 
            // textBox_AdminPortalServer
            // 
            this.textBox_AdminPortalServer.Location = new System.Drawing.Point(190, 259);
            this.textBox_AdminPortalServer.Name = "textBox_AdminPortalServer";
            this.textBox_AdminPortalServer.Size = new System.Drawing.Size(306, 24);
            this.textBox_AdminPortalServer.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(23, 225);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "AZURE SUBSCRIPTION";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(190, 221);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(306, 24);
            this.textBox1.TabIndex = 10;
            // 
            // tenantUIPage
            // 
            this.tenantUIPage.Controls.Add(this.tenantPortalLabel);
            this.tenantUIPage.Controls.Add(this.label11);
            this.tenantUIPage.Controls.Add(this.textBox_TenantPortalServer);
            this.tenantUIPage.Controls.Add(this.button2);
            this.tenantUIPage.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.tenantUIPage.Location = new System.Drawing.Point(4, 53);
            this.tenantUIPage.Name = "tenantUIPage";
            this.tenantUIPage.Size = new System.Drawing.Size(600, 551);
            this.tenantUIPage.TabIndex = 2;
            this.tenantUIPage.Text = "TENANT PORTAL UI TEST";
            this.tenantUIPage.UseVisualStyleBackColor = true;
            // 
            // tenantPortalLabel
            // 
            this.tenantPortalLabel.AutoSize = true;
            this.tenantPortalLabel.Location = new System.Drawing.Point(12, 88);
            this.tenantPortalLabel.Name = "tenantPortalLabel";
            this.tenantPortalLabel.Size = new System.Drawing.Size(156, 17);
            this.tenantPortalLabel.TabIndex = 21;
            this.tenantPortalLabel.Text = "TENANT PORTAL UI TESTS";
            this.tenantPortalLabel.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(8, 41);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(133, 15);
            this.label11.TabIndex = 20;
            this.label11.Text = "TENANT PORTAL SERVER";
            // 
            // textBox_TenantPortalServer
            // 
            this.textBox_TenantPortalServer.Location = new System.Drawing.Point(154, 37);
            this.textBox_TenantPortalServer.Name = "textBox_TenantPortalServer";
            this.textBox_TenantPortalServer.Size = new System.Drawing.Size(290, 24);
            this.textBox_TenantPortalServer.TabIndex = 15;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Font = new System.Drawing.Font("Calibri", 9F);
            this.checkBox2.Location = new System.Drawing.Point(460, 203);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(129, 18);
            this.checkBox2.TabIndex = 34;
            this.checkBox2.Text = "VIEW IN PLAIN TEXT";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(8, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 15);
            this.label10.TabIndex = 15;
            this.label10.Text = "PASSWORD";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "bad.png");
            this.imageList1.Images.SetKeyName(1, "good.png");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(608, 608);
            this.Controls.Add(this.tabControlTesting);
            this.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Phoenix Test Tool";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControlTesting.ResumeLayout(false);
            this.postInstallPage.ResumeLayout(false);
            this.postInstallPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.allTestsPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infraCheckPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resourceProviderPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webappsPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.servicesPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.databasesPic)).EndInit();
            this.adminUIPage.ResumeLayout(false);
            this.adminUIPage.PerformLayout();
            this.tenantUIPage.ResumeLayout(false);
            this.tenantUIPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox_ClientId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_TestPlan;
        private System.Windows.Forms.TextBox textBox_ClientKey;
        private System.Windows.Forms.TextBox textBox_TenantId;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox_ServerName;
        private System.Windows.Forms.TabControl tabControlTesting;
        private System.Windows.Forms.TabPage postInstallPage;
        private System.Windows.Forms.TabPage adminUIPage;
        private System.Windows.Forms.TabPage tenantUIPage;
        private System.Windows.Forms.Label lblCheckWebApps;
        private System.Windows.Forms.Label lblCheckServices;
        private System.Windows.Forms.Label lblCheckDatabases;
        private System.Windows.Forms.Label lblPostInstallSteps;
        private System.Windows.Forms.ProgressBar cmpwapProgressBar;
        private System.Windows.Forms.PictureBox webappsPic;
        private System.Windows.Forms.PictureBox servicesPic;
        private System.Windows.Forms.PictureBox databasesPic;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_AdminPortalServer;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox_TenantPortalServer;
        private System.Windows.Forms.PictureBox resourceProviderPic;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblCheckResourceProvider;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label lblCheckInfra;
        private System.Windows.Forms.TextBox tenantUserPsw;
        private System.Windows.Forms.TextBox tenantUserAccount;
        private System.Windows.Forms.PictureBox infraCheckPic;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.PictureBox allTestsPic;
        private System.Windows.Forms.Label lblAllTests;
        private System.Windows.Forms.ComboBox regions;
        private System.Windows.Forms.Label adminPortalLabel;
        private System.Windows.Forms.Label tenantPortalLabel;
    }
}

