namespace CmpWorkerServiceTestUI
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
            this.button_OneRun = new System.Windows.Forms.Button();
            this.button_RunAsService = new System.Windows.Forms.Button();
            this.button_Close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_OneRun
            // 
            this.button_OneRun.Location = new System.Drawing.Point(51, 33);
            this.button_OneRun.Name = "button_OneRun";
            this.button_OneRun.Size = new System.Drawing.Size(159, 23);
            this.button_OneRun.TabIndex = 0;
            this.button_OneRun.Text = "One Run";
            this.button_OneRun.UseVisualStyleBackColor = true;
            this.button_OneRun.Click += new System.EventHandler(this.button_OneRun_Click);
            // 
            // button_RunAsService
            // 
            this.button_RunAsService.Location = new System.Drawing.Point(51, 85);
            this.button_RunAsService.Name = "button_RunAsService";
            this.button_RunAsService.Size = new System.Drawing.Size(159, 23);
            this.button_RunAsService.TabIndex = 1;
            this.button_RunAsService.Text = "Run As Service";
            this.button_RunAsService.UseVisualStyleBackColor = true;
            this.button_RunAsService.Click += new System.EventHandler(this.button_RunAsService_Click);
            // 
            // button_Close
            // 
            this.button_Close.Location = new System.Drawing.Point(155, 218);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(102, 23);
            this.button_Close.TabIndex = 2;
            this.button_Close.Text = "Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.button_RunAsService);
            this.Controls.Add(this.button_OneRun);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_OneRun;
        private System.Windows.Forms.Button button_RunAsService;
        private System.Windows.Forms.Button button_Close;
    }
}

