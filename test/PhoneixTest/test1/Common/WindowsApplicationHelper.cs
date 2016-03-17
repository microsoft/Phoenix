namespace Phoenix.Test.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public static class WindowsApplicationHelper
    {

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", SetLastError=true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        public static void Login(string windowTitle, string userName, string password)
        {
            IntPtr hWnd = WindowsApplicationHelper.FindWindow(null, windowTitle);
            while (hWnd == null)
            {
                System.Threading.Thread.Sleep(500);
                hWnd = WindowsApplicationHelper.FindWindow(null, windowTitle);
            }

            WindowsApplicationHelper.SetForegroundWindow(hWnd);
            SendKeys.SendWait(userName);  System.Threading.Thread.Sleep(2000);
            SendKeys.SendWait("{TAB}"); System.Threading.Thread.Sleep(2000);
            SendKeys.SendWait(password); System.Threading.Thread.Sleep(2000);
            SendKeys.SendWait("{ENTER}");
        }

        public static void Login0(string windowTitle, string userName, string password)
        {
            IntPtr hWnd = WindowsApplicationHelper.FindWindow(null, windowTitle);
            while (hWnd == null)
            {
                System.Threading.Thread.Sleep(500);
                hWnd = WindowsApplicationHelper.FindWindow(null, windowTitle);
            }

            WindowsApplicationHelper.SetForegroundWindow(hWnd);
            SendKeys.SendWait(userName); System.Threading.Thread.Sleep(2000);
            SendKeys.SendWait("{TAB}"); System.Threading.Thread.Sleep(2000);
            SendKeys.SendWait(password); System.Threading.Thread.Sleep(2000);
            SendKeys.SendWait("{ENTER}");
        }
    }
}
