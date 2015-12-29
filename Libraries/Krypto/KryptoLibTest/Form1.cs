using KryptoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KryptoLibTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_RoundTrip_Click(object sender, EventArgs e)
        {
            try
            {
                ///X509Krypto XK = new X509Krypto( "My", "LocalMachine", "339C80283637D69D9D9F0F5FE00970D93CB0A148");
                X509Krypto XK = new X509Krypto( StoreName_t.Text, StoreLocation_t.Text, CertThumbprint_t.Text);

                CipherText_t.Text = XK.Encrypt(ClearText_t.Text);
                ClearText_t.Text = XK.Decrypt(CipherText_t.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_Encrypt_Click(object sender, EventArgs e)
        {
            X509Krypto XK = new X509Krypto(StoreName_t.Text, StoreLocation_t.Text, CertThumbprint_t.Text);

            CipherText_t.Text = XK.Encrypt(ClearText_t.Text);
        }

        private void button_Decrypt_Click(object sender, EventArgs e)
        {
            X509Krypto XK = new X509Krypto(StoreName_t.Text, StoreLocation_t.Text, CertThumbprint_t.Text);

            ClearText_t.Text = XK.Decrypt(CipherText_t.Text);
        }
    }
}
