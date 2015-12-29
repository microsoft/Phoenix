using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PowerShellLibTestUI
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

        private void button_Test_Click(object sender, EventArgs e)
        {
            PowershellLib.RemotingResult RR = null;

            try
            {
                string Line;

                Output_lb.Items.Clear();

                PowershellLib.Remoting Rem = new PowershellLib.Remoting(Host_t.Text, Name_t.Text, Password_t.Text);

                //PowershellLib.RemotingResult RR = Rem.Execute(Host_t.Text, Directory_t.Text, Command_t.Text);

                /*if (Impersonate_cb.Checked)
                {
                    using (new Impersonator(Name_t.Text, Domain_t.Text, Password_t.Text))
                    {
                        RR = Rem.Execute(Host_t.Text, Directory_t.Text, new List<string>(Command_t.Text.Split(new char[] { '\r', '\n' })));
                    }
                }
                else
                {
                    RR = Rem.Execute(Host_t.Text, Directory_t.Text, new List<string>(Command_t.Text.Split(new char[] { '\r', '\n' })));
                }*/


                //***

                RR = Rem.Execute(Host_t.Text, Directory_t.Text, new List<string>(Command_t.Text.Split(new char[] { '\r', '\n' })),"markwes","123Mainau");

                //***


                if (RR.HasErrors)
                {
                    foreach (object Error in RR.ErrorList)
                    {
                        Line = Convert.ToString(Error);
                        Output_lb.Items.Add(Line);
                    }

                    MessageBox.Show("Command execution errors", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    foreach (object Thing in RR.Output)
                    {
                        Line = Convert.ToString(Thing);
                        Output_lb.Items.Add(Line);
                    }

                    MessageBox.Show("Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button_AzureSpecial_Click(object sender, EventArgs e)
        {
            string publishSettingsPath = "C:\\Data\\AzureMarkWes.publishsettings";
            //string subscriptionName = "MarkWestMsftInternal";
            //string cloudServiceName = "RRRRRRT";
            //string virtualMachineName = "RRRRRRTRN";
            //string cloudServiceName = "Rabbitt";
            //string virtualMachineName = "RabbittRN";
            try
            {
                PowershellLib.VirtualMachineRemotePowerShell.Enable(publishSettingsPath, 
                    SubscriptionName_t.Text, ServiceName_t.Text, VmName_t.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
