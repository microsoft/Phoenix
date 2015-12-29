using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AzureAdminClientLib;
using CmpCommon;
using JsonPrettyPrinterPlus;
using JsonPrettyPrinterPlus.JsonPrettyPrinterInternals;

namespace ArmAccessTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Connection GetConnection()
        {
            try
            {
                return new Connection( AzureSubId_t.Text, null, 
                    AadTenantId_t.Text, AadClientId_t.Text, AadClientKey_t.Text);
            }
            catch (Exception ex)
            {
                Application.UseWaitCursor = false;
                MessageBox.Show("Exception while fetching AAD token : " + Utilities.UnwindExceptionMessages(ex),
                    "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }
        }

        private void FetchArmResourceGroupList()
        {
        }

        private void button_FetchResourceGroupList_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Application.UseWaitCursor = true;

            var conn = GetConnection();

            if (null == conn)
            {
                Application.UseWaitCursor = false;
                return;
            }

            var hso = new HostedServiceOps(conn);
            var res = hso.GetResourceGroupList();

            if (res.HadError)
            {
                Application.UseWaitCursor = false;
                MessageBox.Show("Exception while fetching resource group list : " + res.Body,
                    "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            var pp = new JsonPrettyPrinter(new JsonPPStrategyContext());
            Output_rtb.Text = pp.PrettyPrint(res.Body);

            Application.UseWaitCursor = false;
            MessageBox.Show("Successfully connected to ARM",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
