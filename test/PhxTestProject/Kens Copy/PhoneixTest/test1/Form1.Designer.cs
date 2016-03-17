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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox_ClientId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_TestPlan = new System.Windows.Forms.Button();
            this.textBox_ClientKey = new System.Windows.Forms.TextBox();
            this.textBox_TenantId = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 196);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 21);
            this.button1.TabIndex = 0;
            this.button1.Text = "Test Admin Portal";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(14, 257);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(133, 21);
            this.button2.TabIndex = 1;
            this.button2.Text = "Test Tenant Portal";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox_ClientId
            // 
            this.textBox_ClientId.Location = new System.Drawing.Point(79, 78);
            this.textBox_ClientId.Name = "textBox_ClientId";
            this.textBox_ClientId.Size = new System.Drawing.Size(263, 20);
            this.textBox_ClientId.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Tenant ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Client ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Client Key";
            // 
            // btn_TestPlan
            // 
            this.btn_TestPlan.Location = new System.Drawing.Point(12, 12);
            this.btn_TestPlan.Name = "btn_TestPlan";
            this.btn_TestPlan.Size = new System.Drawing.Size(133, 22);
            this.btn_TestPlan.TabIndex = 2;
            this.btn_TestPlan.Text = "Test Plan";
            this.btn_TestPlan.UseVisualStyleBackColor = true;
            this.btn_TestPlan.Click += new System.EventHandler(this.btn_TestPlan_Click);
            // 
            // textBox_ClientKey
            // 
            this.textBox_ClientKey.Location = new System.Drawing.Point(79, 116);
            this.textBox_ClientKey.Name = "textBox_ClientKey";
            this.textBox_ClientKey.Size = new System.Drawing.Size(263, 20);
            this.textBox_ClientKey.TabIndex = 8;
            // 
            // textBox_TenantId
            // 
            this.textBox_TenantId.Location = new System.Drawing.Point(79, 157);
            this.textBox_TenantId.Name = "textBox_TenantId";
            this.textBox_TenantId.Size = new System.Drawing.Size(263, 20);
            this.textBox_TenantId.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(583, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "---------------------------------------------------------------------------------" +
    "--------------------------------------------------------------------------------" +
    "-------------------------------";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 232);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(583, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "---------------------------------------------------------------------------------" +
    "--------------------------------------------------------------------------------" +
    "-------------------------------";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 311);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_TenantId);
            this.Controls.Add(this.textBox_ClientKey);
            this.Controls.Add(this.btn_TestPlan);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_ClientId);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Name = "Form1";
            this.Text = "Phoenix Test Tool";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

