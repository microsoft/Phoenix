namespace ArmAccessTools
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.AzureSubId_t = new System.Windows.Forms.TextBox();
            this.AadTenantId_t = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AadClientId_t = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AadClientKey_t = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Output_rtb = new System.Windows.Forms.RichTextBox();
            this.button_Close = new System.Windows.Forms.Button();
            this.button_FetchResourceGroupList = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Azure Subscription:";
            // 
            // AzureSubId_t
            // 
            this.AzureSubId_t.Location = new System.Drawing.Point(196, 34);
            this.AzureSubId_t.Name = "AzureSubId_t";
            this.AzureSubId_t.Size = new System.Drawing.Size(345, 26);
            this.AzureSubId_t.TabIndex = 1;
            // 
            // AadTenantId_t
            // 
            this.AadTenantId_t.Location = new System.Drawing.Point(196, 98);
            this.AadTenantId_t.Name = "AadTenantId_t";
            this.AadTenantId_t.Size = new System.Drawing.Size(345, 26);
            this.AadTenantId_t.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "AAD Tenant ID:";
            // 
            // AadClientId_t
            // 
            this.AadClientId_t.Location = new System.Drawing.Point(196, 161);
            this.AadClientId_t.Name = "AadClientId_t";
            this.AadClientId_t.Size = new System.Drawing.Size(345, 26);
            this.AadClientId_t.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "AAD Client ID:";
            // 
            // AadClientKey_t
            // 
            this.AadClientKey_t.Location = new System.Drawing.Point(196, 223);
            this.AadClientKey_t.Name = "AadClientKey_t";
            this.AadClientKey_t.Size = new System.Drawing.Size(345, 26);
            this.AadClientKey_t.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 229);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "AAD Client Key:";
            // 
            // Output_rtb
            // 
            this.Output_rtb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output_rtb.Location = new System.Drawing.Point(47, 339);
            this.Output_rtb.Name = "Output_rtb";
            this.Output_rtb.Size = new System.Drawing.Size(855, 416);
            this.Output_rtb.TabIndex = 8;
            this.Output_rtb.Text = "";
            // 
            // button_Close
            // 
            this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Close.Location = new System.Drawing.Point(814, 781);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(88, 31);
            this.button_Close.TabIndex = 9;
            this.button_Close.Text = "Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // button_FetchResourceGroupList
            // 
            this.button_FetchResourceGroupList.Location = new System.Drawing.Point(588, 218);
            this.button_FetchResourceGroupList.Name = "button_FetchResourceGroupList";
            this.button_FetchResourceGroupList.Size = new System.Drawing.Size(80, 36);
            this.button_FetchResourceGroupList.TabIndex = 10;
            this.button_FetchResourceGroupList.Text = "Test";
            this.button_FetchResourceGroupList.UseVisualStyleBackColor = true;
            this.button_FetchResourceGroupList.Click += new System.EventHandler(this.button_FetchResourceGroupList_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(47, 296);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = "Resource Group List:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(570, 34);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(332, 131);
            this.textBox1.TabIndex = 13;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 834);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_FetchResourceGroupList);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.Output_rtb);
            this.Controls.Add(this.AadClientKey_t);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.AadClientId_t);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AadTenantId_t);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AzureSubId_t);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Azure ARM Access Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox AzureSubId_t;
        private System.Windows.Forms.TextBox AadTenantId_t;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox AadClientId_t;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AadClientKey_t;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox Output_rtb;
        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.Button button_FetchResourceGroupList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
    }
}

