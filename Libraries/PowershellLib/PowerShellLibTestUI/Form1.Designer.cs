namespace PowerShellLibTestUI
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
            this.button_Close = new System.Windows.Forms.Button();
            this.button_Test = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Host_t = new System.Windows.Forms.TextBox();
            this.Command_t = new System.Windows.Forms.TextBox();
            this.Output_lb = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Directory_t = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Impersonate_cb = new System.Windows.Forms.CheckBox();
            this.Password_t = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Name_t = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Domain_t = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button_AzureSpecial = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.VmName_t = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ServiceName_t = new System.Windows.Forms.TextBox();
            this.l3 = new System.Windows.Forms.Label();
            this.SubscriptionName_t = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Close
            // 
            this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Close.Location = new System.Drawing.Point(596, 448);
            this.button_Close.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(56, 33);
            this.button_Close.TabIndex = 0;
            this.button_Close.Text = "Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // button_Test
            // 
            this.button_Test.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Test.Location = new System.Drawing.Point(507, 448);
            this.button_Test.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_Test.Name = "button_Test";
            this.button_Test.Size = new System.Drawing.Size(63, 33);
            this.button_Test.TabIndex = 1;
            this.button_Test.Text = "Execute";
            this.button_Test.UseVisualStyleBackColor = true;
            this.button_Test.Click += new System.EventHandler(this.button_Test_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Host:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Command:";
            // 
            // Host_t
            // 
            this.Host_t.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Host_t.Location = new System.Drawing.Point(94, 24);
            this.Host_t.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Host_t.Name = "Host_t";
            this.Host_t.Size = new System.Drawing.Size(558, 20);
            this.Host_t.TabIndex = 4;
            this.Host_t.Text = "tk5-cu-vmm05";
            // 
            // Command_t
            // 
            this.Command_t.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Command_t.Location = new System.Drawing.Point(94, 88);
            this.Command_t.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Command_t.Multiline = true;
            this.Command_t.Name = "Command_t";
            this.Command_t.Size = new System.Drawing.Size(558, 50);
            this.Command_t.TabIndex = 5;
            this.Command_t.Text = ".\\\\VmProcessorTest_Success -ID 25 -CallbackURL http://tk5mmm1/CMP/CMP/FLUs -Sourc" +
    "eServer TheSourceServer -TargetVmName TheTargetVmName";
            // 
            // Output_lb
            // 
            this.Output_lb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output_lb.FormattingEnabled = true;
            this.Output_lb.Location = new System.Drawing.Point(25, 236);
            this.Output_lb.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Output_lb.Name = "Output_lb";
            this.Output_lb.Size = new System.Drawing.Size(628, 108);
            this.Output_lb.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 56);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Directory:";
            // 
            // Directory_t
            // 
            this.Directory_t.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Directory_t.Location = new System.Drawing.Point(94, 54);
            this.Directory_t.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Directory_t.Name = "Directory_t";
            this.Directory_t.Size = new System.Drawing.Size(558, 20);
            this.Directory_t.TabIndex = 8;
            this.Directory_t.Text = "D:\\FLUMigrationModules";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Impersonate_cb);
            this.groupBox1.Controls.Add(this.Password_t);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.Name_t);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.Domain_t);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(25, 150);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(627, 65);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Impersonation";
            // 
            // Impersonate_cb
            // 
            this.Impersonate_cb.AutoSize = true;
            this.Impersonate_cb.Location = new System.Drawing.Point(14, 26);
            this.Impersonate_cb.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Impersonate_cb.Name = "Impersonate_cb";
            this.Impersonate_cb.Size = new System.Drawing.Size(98, 19);
            this.Impersonate_cb.TabIndex = 6;
            this.Impersonate_cb.Text = "Impersonate";
            this.Impersonate_cb.UseVisualStyleBackColor = true;
            // 
            // Password_t
            // 
            this.Password_t.Location = new System.Drawing.Point(514, 24);
            this.Password_t.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Password_t.Name = "Password_t";
            this.Password_t.PasswordChar = '*';
            this.Password_t.Size = new System.Drawing.Size(102, 20);
            this.Password_t.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(455, 27);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "Password:";
            // 
            // Name_t
            // 
            this.Name_t.Location = new System.Drawing.Point(345, 24);
            this.Name_t.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name_t.Name = "Name_t";
            this.Name_t.Size = new System.Drawing.Size(102, 20);
            this.Name_t.TabIndex = 3;
            this.Name_t.Text = "_hstools";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(296, 27);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Name:";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // Domain_t
            // 
            this.Domain_t.Location = new System.Drawing.Point(183, 24);
            this.Domain_t.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Domain_t.Name = "Domain_t";
            this.Domain_t.Size = new System.Drawing.Size(102, 20);
            this.Domain_t.TabIndex = 1;
            this.Domain_t.Text = "redmond";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(134, 27);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Domain:";
            // 
            // button_AzureSpecial
            // 
            this.button_AzureSpecial.Location = new System.Drawing.Point(509, 55);
            this.button_AzureSpecial.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_AzureSpecial.Name = "button_AzureSpecial";
            this.button_AzureSpecial.Size = new System.Drawing.Size(106, 19);
            this.button_AzureSpecial.TabIndex = 10;
            this.button_AzureSpecial.Text = "Azure Special";
            this.button_AzureSpecial.UseVisualStyleBackColor = true;
            this.button_AzureSpecial.Click += new System.EventHandler(this.button_AzureSpecial_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.VmName_t);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.ServiceName_t);
            this.groupBox2.Controls.Add(this.l3);
            this.groupBox2.Controls.Add(this.SubscriptionName_t);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.button_AzureSpecial);
            this.groupBox2.Location = new System.Drawing.Point(25, 358);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(627, 86);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Azure Special Test";
            // 
            // VmName_t
            // 
            this.VmName_t.Location = new System.Drawing.Point(460, 26);
            this.VmName_t.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.VmName_t.Name = "VmName_t";
            this.VmName_t.Size = new System.Drawing.Size(91, 20);
            this.VmName_t.TabIndex = 16;
            this.VmName_t.Text = "NewDaySS13";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(404, 28);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 15);
            this.label8.TabIndex = 15;
            this.label8.Text = "VM Name";
            // 
            // ServiceName_t
            // 
            this.ServiceName_t.Location = new System.Drawing.Point(300, 26);
            this.ServiceName_t.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ServiceName_t.Name = "ServiceName_t";
            this.ServiceName_t.Size = new System.Drawing.Size(91, 20);
            this.ServiceName_t.TabIndex = 14;
            this.ServiceName_t.Text = "NewDaySS13";
            // 
            // l3
            // 
            this.l3.AutoSize = true;
            this.l3.Location = new System.Drawing.Point(224, 28);
            this.l3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.l3.Name = "l3";
            this.l3.Size = new System.Drawing.Size(84, 15);
            this.l3.TabIndex = 13;
            this.l3.Text = "Service Name";
            // 
            // SubscriptionName_t
            // 
            this.SubscriptionName_t.Location = new System.Drawing.Point(87, 26);
            this.SubscriptionName_t.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.SubscriptionName_t.Name = "SubscriptionName_t";
            this.SubscriptionName_t.Size = new System.Drawing.Size(125, 20);
            this.SubscriptionName_t.TabIndex = 12;
            this.SubscriptionName_t.TabStop = false;
            this.SubscriptionName_t.Text = "MarkWestMsftInternal";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 28);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 15);
            this.label7.TabIndex = 11;
            this.label7.Text = "Subscription:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 499);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Directory_t);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Output_lb);
            this.Controls.Add(this.Command_t);
            this.Controls.Add(this.Host_t);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Test);
            this.Controls.Add(this.button_Close);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Powershell Remoting Test UI";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.Button button_Test;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Host_t;
        private System.Windows.Forms.TextBox Command_t;
        private System.Windows.Forms.ListBox Output_lb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Directory_t;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox Password_t;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Name_t;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Domain_t;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox Impersonate_cb;
        private System.Windows.Forms.Button button_AzureSpecial;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox ServiceName_t;
        private System.Windows.Forms.Label l3;
        private System.Windows.Forms.TextBox SubscriptionName_t;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox VmName_t;
        private System.Windows.Forms.Label label8;
    }
}

