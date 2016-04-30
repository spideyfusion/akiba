using Akiba.Core;
using Akiba.Core.Exceptions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Akiba
{
    class Program
    {
        public static Configuration Config;

        private enum ExitCodes : int
        {
            Success,
            BoostrapFail,
            ConfigurationError,
            FramerateAlterFail,
            UnhandledException,
            UserCanceled,
        }

        [STAThread]
        private static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            try
            {
                if (!PerformBootstrap())
                {
                    return (int)ExitCodes.UserCanceled;
                }
            }
            catch (BootstrapException e)
            {
                MessageBox.Show(
                    e.Message,
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return (int)ExitCodes.BoostrapFail;
            }

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

            using (var gameSettings = new AkibaSettings())
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
                UseShellExecute = false, // Spawn the process directly via the CreateProcess() API call to avoid issues with security software.
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

        private static bool PerformBootstrap()
        {
            var installer = new AkibaInstaller();

            AkibaInstaller.InstallStatus installStatus = installer.Install();

            if (installStatus == AkibaInstaller.InstallStatus.UpgradeRequested)
            {
                DialogResult dialogResult = MessageBox.Show(
                    string.Format(Properties.Resources.MessageUpgradeNotice, Application.ProductName, installer.OldUtility.ProductVersion, Application.ProductVersion),
                    Application.ProductName,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (dialogResult == DialogResult.Yes)
                {
                    installer.Upgrade();
                }
                else
                {
                    return false;
                }
            }
            else if (installStatus == AkibaInstaller.InstallStatus.Installed)
            {
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
                Utilities.LaunchSteamGame(
                    Utilities.GetSteamApplicationId(Assembly.GetExecutingAssembly())
                );
            }
        }

        private static void StartGameMonitorLoop()
        {
            Process gameProcess = Utilities.GetGameProcess();

            do
            {
                // Let's just wait for a little bit...
                Task.Delay(5000).Wait();

                gameProcess.Refresh();
            } while (gameProcess.MainWindowHandle == IntPtr.Zero);

            IntPtr gameWindow = gameProcess.MainWindowHandle;

            // Make sure the game window is visible before we start manipulating it.
            NativeMethods.SendMessage(gameWindow, NativeMethods.WM_SYSCOMMAND, (IntPtr)NativeMethods.SC_RESTORE, IntPtr.Zero);

            if (Config.ScreenMode == Configuration.ScreenModes.Borderless)
            {
                ScreenManager.OccupyScreen(gameWindow, ScreenManager.GetGameScreen(gameWindow));
            }

            if (Config.HideCursor)
            {
                // Move the cursor off the screen in case people are launching the game from Big Picture.
                Cursor.Position = new Point(SystemInformation.VirtualScreen.Right, SystemInformation.VirtualScreen.Bottom);
            }

            if (Config.PreventSystemSleep)
            {
                // Prevent the system from going to sleep if the game is running.
                NativeMethods.SetThreadExecutionState(NativeMethods.ExecutionStateFlags.Continuous | NativeMethods.ExecutionStateFlags.DisplayRequired);
            }

            gameProcess.WaitForExit();
        }
    }
}
