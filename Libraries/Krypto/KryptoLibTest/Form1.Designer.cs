namespace KryptoLibTest
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
            this.button_RoundTrip = new System.Windows.Forms.Button();
            this.button_Close = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CertThumbprint_t = new System.Windows.Forms.TextBox();
            this.StoreLocation_t = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.StoreName_t = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ClearText_t = new System.Windows.Forms.TextBox();
            this.edfer = new System.Windows.Forms.Label();
            this.CipherText_t = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button_Decrypt = new System.Windows.Forms.Button();
            this.button_Encrypt = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_RoundTrip
            // 
            this.button_RoundTrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_RoundTrip.Location = new System.Drawing.Point(331, 263);
            this.button_RoundTrip.Name = "button_RoundTrip";
            this.button_RoundTrip.Size = new System.Drawing.Size(75, 23);
            this.button_RoundTrip.TabIndex = 0;
            this.button_RoundTrip.Text = "Round Trip";
            this.button_RoundTrip.UseVisualStyleBackColor = true;
            this.button_RoundTrip.Click += new System.EventHandler(this.button_RoundTrip_Click);
            // 
            // button_Close
            // 
            this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Close.Location = new System.Drawing.Point(431, 263);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(77, 23);
            this.button_Close.TabIndex = 1;
            this.button_Close.Text = "Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Cert Thumbprint";
            // 
            // CertThumbprint_t
            // 
            this.CertThumbprint_t.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CertThumbprint_t.Location = new System.Drawing.Point(120, 30);
            this.CertThumbprint_t.Name = "CertThumbprint_t";
            this.CertThumbprint_t.Size = new System.Drawing.Size(376, 20);
            this.CertThumbprint_t.TabIndex = 3;
            this.CertThumbprint_t.Text = "0ae99f687b48923f9b79755eeb51ea683e143ed4";
            // 
            // StoreLocation_t
            // 
            this.StoreLocation_t.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StoreLocation_t.Location = new System.Drawing.Point(120, 65);
            this.StoreLocation_t.Name = "StoreLocation_t";
            this.StoreLocation_t.Size = new System.Drawing.Size(375, 20);
            this.StoreLocation_t.TabIndex = 5;
            this.StoreLocation_t.Text = "LocalMachine";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Store Location:";
            // 
            // StoreName_t
            // 
            this.StoreName_t.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StoreName_t.Location = new System.Drawing.Point(120, 103);
            this.StoreName_t.Name = "StoreName_t";
            this.StoreName_t.Size = new System.Drawing.Size(375, 20);
            this.StoreName_t.TabIndex = 7;
            this.StoreName_t.Text = "My";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Store Name:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.CertThumbprint_t);
            this.groupBox1.Controls.Add(this.StoreName_t);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.StoreLocation_t);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(509, 144);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Certificate";
            // 
            // ClearText_t
            // 
            this.ClearText_t.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearText_t.Location = new System.Drawing.Point(132, 181);
            this.ClearText_t.Name = "ClearText_t";
            this.ClearText_t.Size = new System.Drawing.Size(376, 20);
            this.ClearText_t.TabIndex = 10;
            // 
            // edfer
            // 
            this.edfer.AutoSize = true;
            this.edfer.Location = new System.Drawing.Point(21, 184);
            this.edfer.Name = "edfer";
            this.edfer.Size = new System.Drawing.Size(58, 13);
            this.edfer.TabIndex = 9;
            this.edfer.Text = "Clear Text:";
            // 
            // CipherText_t
            // 
            this.CipherText_t.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CipherText_t.Location = new System.Drawing.Point(132, 223);
            this.CipherText_t.Name = "CipherText_t";
            this.CipherText_t.Size = new System.Drawing.Size(376, 20);
            this.CipherText_t.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 226);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Cipher Text:";
            // 
            // button_Decrypt
            // 
            this.button_Decrypt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Decrypt.Location = new System.Drawing.Point(241, 263);
            this.button_Decrypt.Name = "button_Decrypt";
            this.button_Decrypt.Size = new System.Drawing.Size(75, 23);
            this.button_Decrypt.TabIndex = 13;
            this.button_Decrypt.Text = "Decrypt";
            this.button_Decrypt.UseVisualStyleBackColor = true;
            this.button_Decrypt.Click += new System.EventHandler(this.button_Decrypt_Click);
            // 
            // button_Encrypt
            // 
            this.button_Encrypt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Encrypt.Location = new System.Drawing.Point(149, 263);
            this.button_Encrypt.Name = "button_Encrypt";
            this.button_Encrypt.Size = new System.Drawing.Size(75, 23);
            this.button_Encrypt.TabIndex = 14;
            this.button_Encrypt.Text = "Encrypt";
            this.button_Encrypt.UseVisualStyleBackColor = true;
            this.button_Encrypt.Click += new System.EventHandler(this.button_Encrypt_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 298);
            this.Controls.Add(this.button_Encrypt);
            this.Controls.Add(this.button_Decrypt);
            this.Controls.Add(this.CipherText_t);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ClearText_t);
            this.Controls.Add(this.edfer);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.button_RoundTrip);
            this.Name = "Form1";
            this.Text = "KText Encryptor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_RoundTrip;
        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CertThumbprint_t;
        private System.Windows.Forms.TextBox StoreLocation_t;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox StoreName_t;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox ClearText_t;
        private System.Windows.Forms.Label edfer;
        private System.Windows.Forms.TextBox CipherText_t;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_Decrypt;
        private System.Windows.Forms.Button button_Encrypt;
    }
}

