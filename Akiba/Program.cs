using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Akiba
{
    class Program
    {
        public static Configuration Config;

        private enum ExitCodes : int
        {
            Success = 0,
            BoostrapFail,
            ConfigurationError,
            FramerateAlterFail,
            UnhandledException,
        }

        [STAThread]
        private static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            Config = InitializeConfiguration();

            if (Config == null)
            {
                MessageBox.Show(
                    Properties.Resources.MessageConfigurationError,
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return (int)ExitCodes.ConfigurationError;
            }

            if (!InitialBootstrap())
            {
                return (int)ExitCodes.BoostrapFail;
            }

            switch (args.DefaultIfEmpty(null).First())
            {
                case Utilities.MonitorTriggerSwitch:
                    StartGameMonitorLoop();
                    return (int)ExitCodes.Success;
                case Utilities.ConfigTriggerSwitch:
                    // We got called by the game!
                    break;
                default:
                    LaunchConfigurationUtility();
                    return (int)ExitCodes.Success;
            }

            using (AkibaSettings gameSettings = new AkibaSettings())
            {
                gameSettings.ApplySettings();
            }

            if (!Utilities.AlterGameFramerate())
            {
                MessageBox.Show(
                    Properties.Resources.MessageFramerateError,
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return (int)ExitCodes.FramerateAlterFail;
            }

            // We're going to re-launch ourselves and monitor the game.
            Process.Start(new ProcessStartInfo
            {
                FileName = Utilities.ConfigExecutableName,
                Arguments = Utilities.MonitorTriggerSwitch,
            });

            return (int)ExitCodes.Success;
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                e.ExceptionObject.ToString(),
                Application.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            Environment.Exit((int)ExitCodes.UnhandledException);
        }

        private static bool InitialBootstrap()
        {
            if (!File.Exists(Utilities.GameExecutableName))
            {
                MessageBox.Show(
                    Properties.Resources.MessageGameLocation,
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return false;
            }

            string currentProcessName = Process.GetCurrentProcess().MainModule.FileName;

            if (Path.GetFileName(currentProcessName).Equals(Utilities.ConfigExecutableName))
            {
                if (!File.Exists(Utilities.BackupConfigExecutableName))
                {
                    MessageBox.Show(
                        Properties.Resources.MessageBackupMissing,
                        Application.ProductName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return false;
                }
            }
            else
            {
                if (!File.Exists(Utilities.ConfigExecutableName))
                {
                    MessageBox.Show(
                        Properties.Resources.MessageConfigUtilityMissing,
                        Application.ProductName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return false;
                }

                FileVersionInfo utilityVersion = FileVersionInfo.GetVersionInfo(Utilities.ConfigExecutableName);

                if (IsUtilityOurOwn(utilityVersion))
                {
                    DialogResult dialogResult = MessageBox.Show(
                        string.Format(Properties.Resources.MessageUpgradeNotice, Application.ProductName, utilityVersion.ProductVersion, Application.ProductVersion),
                        Application.ProductName,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (dialogResult == DialogResult.No)
                    {
                        return false;
                    }

                    // Send the old utility version to oblivion...
                    File.Delete(Utilities.ConfigExecutableName);

                    // And let the new one take its place...
                    File.Move(currentProcessName, Utilities.ConfigExecutableName);

                    return true;
                }

                // Even though we're replacing the config utlity, we don't want to get rid of it.
                File.Move(Utilities.ConfigExecutableName, Utilities.BackupConfigExecutableName);

                // We'll rename our utility to match the game's config executable name.
                File.Move(currentProcessName, Utilities.ConfigExecutableName);

                MessageBox.Show(
                    string.Format(Properties.Resources.MessageSuccess, Configuration.ConfigurationName),
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            return true;
        }

        private static Configuration InitializeConfiguration()
        {
            if (!File.Exists(Configuration.ConfigurationName))
            {
                return new Configuration().Save();
            }

            try
            {
                return Configuration.LoadFromFile().Save();
            }
            catch
            {
                return null;
            }
        }

        private static void LaunchConfigurationUtility()
        {
            Process gameUtility = Process.Start(new ProcessStartInfo
            {
                FileName = Utilities.BackupConfigExecutableName,
                Arguments = Utilities.ConfigTriggerSwitch,
            });

            gameUtility.WaitForExit();

            if (gameUtility.ExitCode == (int)ExitCodes.Success)
            {
                Process.Start(Utilities.GameExecutableName);
            }
        }

        private static void StartGameMonitorLoop()
        {
            Process gameProcess = Utilities.GetGameProcess();

            do
            {
                // Let's just wait for a little bit...
                Task.Delay(1000).Wait();

                gameProcess.Refresh();
            } while (gameProcess.MainWindowHandle == IntPtr.Zero);

            // Move the cursor off the screen in case people are launching the game from Big Picture.
            Cursor.Position = new Point(SystemInformation.VirtualScreen.Right, SystemInformation.VirtualScreen.Bottom);

            if (Config.PreventSystemSleep)
            {
                // Prevent the system from going to sleep if the game is running.
                NativeMethods.SetThreadExecutionState(NativeMethods.ExecutionStateFlags.Continuous | NativeMethods.ExecutionStateFlags.DisplayRequired);
            }

            gameProcess.WaitForExit();
        }

        private static bool IsUtilityOurOwn(FileVersionInfo versionInfo)
        {
            if (!versionInfo.ProductName.Equals(Application.ProductName))
            {
                return false;
            }

            return true;
        }
    }
}
