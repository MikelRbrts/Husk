using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace Husk
{
    

    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern bool ExitWindowsEx(uint uFlags, uint dwReason);
        private string currentSystemShellAppBlt;
        private string currentUserShellAppBlt;
        private string newSystemShellApp;
        private string newUserShellApp;
        private string newSystemShellAppAttrbt;
        private string newUserShellAppAttrbt;
        private string newSystemShellAppBlt;
        private string newUserShellAppBlt;

        public MainWindow() { InitializeComponent(); }

        // Method - On application launch
        private void HuskedUp(object sender, RoutedEventArgs e)
        {
            // Try - System Shell
            try
            {
                RegistryKey regBits = regTyp("HKLM");
                currentSystemShellAppBlt = regBits.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon").GetValue("Shell").ToString();
                currentSystemShellLabel.Content = currentSystemShellAppBlt;
            }
            catch { }
            // Try - User Shell
            try
            {
                RegistryKey regBitTypHKCU = regTyp("HKCU");
                currentUserShellAppBlt = regBitTypHKCU.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon").GetValue("Shell").ToString();
                currentUserShellLabel.Content = currentUserShellAppBlt;
            }
            catch { }
        }

        // Method - Run Command
        void runCommand(string cmdName)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + cmdName;
            process.StartInfo = startInfo;
            process.Start();
        }

        // Method - Determine registry bit type
        private RegistryKey regTyp(string hiveLctn)
        {
            RegistryKey keyBits;
            if (hiveLctn.Equals("HKLM"))
            {
                if (Environment.Is64BitOperatingSystem)
                    keyBits = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    keyBits = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }
            else
            {
                if (Environment.Is64BitOperatingSystem)
                    keyBits = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                else
                    keyBits = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
            }       
            return keyBits;
        }

        // Method - Show file browser and return results
        private string fldrBrwsr()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            string cmdPth = "";
            dlg.DefaultExt = ".exe";
            dlg.Filter = "Executables (*.exe)|*.exe|PowerShell (*.PS1)|*.ps1|VB Scripts (*.vbs, *.wsf)|" +
                "*.vbs,*.wsf|Batch Scripts(*.bat, *.cmd) | *.bat,*.cmd";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true) { cmdPth = dlg.FileName; }
            return cmdPth;
        }

        // Menu - Exit
        private void menuExit(object sender, RoutedEventArgs e) { Environment.Exit(1); }

        // Menu - Run Current System Shell
        private void menuRunSystemShell(object sender, RoutedEventArgs e) { runCommand(currentSystemShellAppBlt); }

        // Menu - Run Current User Shell
        private void menuRunUserShell(object sender, RoutedEventArgs e) { runCommand(currentUserShellAppBlt); }

        // Menu - File Explorer
        private void menuFileExplorer(object sender, RoutedEventArgs e) { System.Diagnostics.Process.Start(@"Explorer.exe"); }

        // Menu - Task Manager
        private void menuTaskManager(object sender, RoutedEventArgs e) { System.Diagnostics.Process.Start(@"Taskmgr.exe"); }

        // Menu - Log Off
        private void menuLogOff(object sender, RoutedEventArgs e) { ExitWindowsEx(0x00, 0x00050000); }

        // Menu - Restart
        private void menuRestart(object sender, RoutedEventArgs e) { ExitWindowsEx(0x02, 0x00050000); }

        // Menu - User Guide
        private void menuUserGuide(object sender, RoutedEventArgs e) { System.Diagnostics.Process.Start("https://github.com/mikelrbrts/husk"); }

        // Menu - Website
        private void menuWebsite(object sender, RoutedEventArgs e) { System.Diagnostics.Process.Start("https://github.com/mikelrbrts/husk"); }

        // Menu - About
        private void menuAbout(object sender, RoutedEventArgs e) { About abtDlg = new About(); abtDlg.ShowDialog(); }

        // Current System Shell - Messagebox - Show Command
        private void currentSystemShellMessage(object sender, RoutedEventArgs e)
        {
            string appRslt;
            if (string.IsNullOrWhiteSpace(currentSystemShellAppBlt))
            { appRslt = "None Set"; }
            else
            { appRslt = currentSystemShellAppBlt; }
            MessageBox.Show("Executable" + "\n\n" + appRslt, "Husk - Shell Configurator");
        }

        // Current User Shell - Messagebox - Show Command
        private void currentUserShellMessage(object sender, RoutedEventArgs e)
        {
            string appRslt;
            if (string.IsNullOrWhiteSpace(currentUserShellAppBlt))
                { appRslt = "None Set"; }
            else
            { appRslt = currentUserShellAppBlt; }
            MessageBox.Show("Executable" + "\n\n" + appRslt, "Husk - Shell Configurator");
        }

        // Change System Shell - TextBox - Update TextBox and Build String on Shell Change
        private void newSystemShell_TextChanged(object sender, TextChangedEventArgs e)
        {
            newSystemShellApp = newSystemShell.Text;
            newSystemShellAppBlt = newSystemShellApp + " " + newSystemShellAppAttrbt;
            newSystemShellStringLabel.Content = newSystemShellAppBlt;
        }

        // Change System Shell - Button - Reset Current Shell
        private void newSystemShellResetBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgRslt = MessageBox.Show(
                "You are about to reset the System Shell back to 'explorer.exe'." +
                "\n\n" + "Would you like to continue?",
                "Husk - Clear System Shell",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);
            if (msgRslt == MessageBoxResult.Yes)
            {
                newSystemShellApp = "explorer.exe";
                newSystemShellAppAttrbt = "";
                newSystemShellAppBlt = newSystemShellApp;
                newSystemShellStringLabel.Content = newSystemShellAppBlt;
            }
        }

        // Change System Shell - Button - New File Browse Button 
        private void newSystemShellBrowseBTN_Click(object sender, RoutedEventArgs e)
        {
            newSystemShellApp = "\"" + fldrBrwsr() + "\"";
            newSystemShellAppBlt = newSystemShellApp + " " + newSystemShellAppAttrbt;
            newSystemShell.Text = newSystemShellApp;
            newSystemShellStringLabel.Content = newSystemShellApp + " " + newSystemShellAppAttrbt;
        }

        // Change System Shell - TextBox - Update TextBox and Build String on Attribute Change
        private void newSystemShellAttributes_TextChanged(object sender, TextChangedEventArgs e)
        {
            newSystemShellAppAttrbt = newSystemShellAttributes.Text;
            newSystemShellAppBlt = newSystemShellApp + " " + newSystemShellAppAttrbt;
            newSystemShellStringLabel.Content = newSystemShellAppBlt;
        }

        // Change System Shell - MessageBox - Show Built Command 
        private void newSystemShellMessage(object sender, RoutedEventArgs e)
        {
            string appRslt;
            if (string.IsNullOrWhiteSpace(newSystemShellApp))
            { appRslt = "None Set"; }
            else
            { appRslt = newSystemShellApp; }
            MessageBox.Show("Executable" + "\n" + appRslt + "\n\n" +
                "Attributes" + "\n" + newSystemShellAppAttrbt, "Husk - Shell Configurator");
        }

        // Change User Shell - TextBox - Update TextBox and Build String on Shell Change
        private void newUserShell_TextChanged(object sender, TextChangedEventArgs e)
        {
            newUserShellApp = newUserShell.Text;
            newUserShellAppBlt = newUserShellApp + " " + newUserShellAppAttrbt;
            newUserShellStringLabel.Content = newUserShellAppBlt;
        }

        // Change User Shell - Button - Reset Current Shell
        private void newUserShellResetBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgRslt = MessageBox.Show(
                "You are about to clear the User Shell entry." +
                "\n\n" + "Would you like to continue?",
                "Husk - Clear User Shell",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (msgRslt == MessageBoxResult.Yes)
            {
                newUserShellApp = "";
                newUserShellAppAttrbt = "";
                newUserShellAppBlt = newUserShellApp;
                newUserShellStringLabel.Content = newUserShellAppBlt;
            }
        }

        // Change User Shell - Button - New File Browse Button
        private void newUserShellBrowseBTN_Click(object sender, RoutedEventArgs e)
        {
            newUserShellApp = "\"" + fldrBrwsr() + "\"";
            newUserShellAppBlt = newUserShellApp + " " + newUserShellAppAttrbt;
            newUserShell.Text = newUserShellApp;
            newUserShellStringLabel.Content = newUserShellApp + " " + newUserShellAppAttrbt;
        }

        // Change User Shell - TextBox - Update TextBox and Build String on Attribute Change
        private void newUserShellAttributes_TextChanged(object sender, TextChangedEventArgs e)
        {
            newUserShellAppAttrbt = newUserShellAttributes.Text;
            newUserShellAppBlt = newUserShellApp + " " + newUserShellAppAttrbt;
            newUserShellStringLabel.Content = newUserShellAppBlt;
        }

        // Change User Shell - MessageBox - Show Built Command 
        private void newUserShellMessage(object sender, RoutedEventArgs e)
        {
            string appRslt;
            if (string.IsNullOrWhiteSpace(newUserShellApp))
            { appRslt = "None Set"; }
            else
            { appRslt = newUserShellApp; }

            MessageBox.Show("Executable" + "\n" + appRslt + "\n\n" +
                "Attributes" + "\n" + newUserShellAppAttrbt, "Husk - Shell Configurator");
        }

        // Button - Save Button
        private void saveClick(object sender, RoutedEventArgs e)
        {
            RegistryKey regHKLM = regTyp("HKLM");
            RegistryKey regHKCU = regTyp("HKCU");
            string changesMadeString = "";
            if (String.IsNullOrEmpty(newSystemShellAppBlt)) { }
            else
            {
                regHKLM = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon");
                regHKLM.SetValue("Shell", newSystemShellAppBlt);
                regHKLM.Close();
                currentSystemShellAppBlt = newSystemShellAppBlt;
                currentSystemShellLabel.Content = newSystemShellAppBlt;
                changesMadeString = "System Shell - " + newSystemShellAppBlt + "\n\n";
            }
            if (String.IsNullOrEmpty(newUserShellAppBlt))
            {
                if (String.IsNullOrEmpty(currentUserShellAppBlt)) { }
                else
                {
                    string keyName = @"Software\Microsoft\Windows NT\CurrentVersion\Winlogon";
                    using (RegistryKey key = regHKCU.OpenSubKey(keyName, true))
                    {
                        if (key == null) { }
                        else
                        {
                            key.DeleteValue("Shell");
                            currentUserShellAppBlt = "";
                            currentUserShellLabel.Content = "";
                            changesMadeString = changesMadeString + "User Shell - Cleared" + "\n\n";
                        }
                    }
                }
            }
            else
            {
                regHKCU = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon");
                regHKCU.SetValue("Shell", newUserShellAppBlt);
                regHKCU.Close();
                currentUserShellAppBlt = newUserShellAppBlt;
                currentUserShellLabel.Content = newUserShellAppBlt;
                changesMadeString = changesMadeString + "User Shell - " + newUserShellAppBlt + "\n\n";
            }
            if (string.IsNullOrWhiteSpace(changesMadeString)) { changesMadeString = "None"; }
            MessageBox.Show(changesMadeString, "Husk - Shell Configurator", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Button - Exit application and drop changes
        private void cancelClick(object sender, RoutedEventArgs e) { System.Environment.Exit(1); }
    }
}
