namespace Akiba
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Akiba.Core;
    using Akiba.Core.Exceptions;

    internal class Program
    {
        public static Configuration Config { get; private set; }

        private enum ExitCodes : int
        {
            Success,
            BoostrapFail,
            ConfigurationError,
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
                _ = MessageBox.Show(
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
                _ = MessageBox.Show(
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

            // We're going to re-launch ourselves and monitor the game.
            _ = Process.Start(new ProcessStartInfo
            {
                FileName = Utilities.ConfigExecutableName,
                Arguments = Utilities.MonitorTriggerSwitch,
                UseShellExecute = false, // Spawn the process directly via the CreateProcess() API call to avoid issues with security software.
            });

            return (int)ExitCodes.Success;
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            _ = MessageBox.Show(
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

            var installStatus = installer.Install();

            if (installStatus == AkibaInstaller.InstallStatus.UpgradeRequested)
            {
                var dialogResult = MessageBox.Show(
                    string.Format(Properties.Resources.MessageUpgradeNotice, Application.ProductName, installer.Utility.ProductVersion, Application.ProductVersion),
                    Application.ProductName,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (dialogResult == DialogResult.Yes)
                {
                    _ = installer.Upgrade();
                }
                else
                {
                    return false;
                }
            }
            else if (installStatus == AkibaInstaller.InstallStatus.Installed)
            {
                _ = MessageBox.Show(
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
            var gameUtility = Process.Start(new ProcessStartInfo
            {
                FileName = Utilities.BackupConfigExecutableName,
                Arguments = Utilities.ConfigTriggerSwitch,
            });

            gameUtility.WaitForExit();

            if (gameUtility.ExitCode == (int)ExitCodes.Success)
            {
                try
                {
                    GameLauncher.Launch();
                }
                catch (UnsupportedGameVariantException)
                {
                    _ = MessageBox.Show(
                        Properties.Resources.MessageUnsupportedGameVariant,
                        Application.ProductName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private static void StartGameMonitorLoop()
        {
            var gameProcess = Utilities.GetGameProcess();

            do
            {
                // Let's just wait for a little bit...
                Task.Delay(5000).Wait();

                gameProcess.Refresh();
            } while (gameProcess.MainWindowHandle == IntPtr.Zero);

            var gameWindow = gameProcess.MainWindowHandle;

            if (Config.ScreenMode == Configuration.ScreenModes.Borderless)
            {
                ScreenManager.OccupyScreen(gameWindow, ScreenManager.GetGameScreen(gameWindow));
            }

            if (Config.HideCursor)
            {
                ScreenManager.HideCuror();
            }

            if (Config.PreventSystemSleep)
            {
                // Prevent the system from going to sleep if the game is running.
                _ = NativeMethods.SetThreadExecutionState(NativeMethods.ExecutionStateFlags.Continuous | NativeMethods.ExecutionStateFlags.DisplayRequired);
            }

            gameProcess.WaitForExit();

            // Restore the original cursor after the game exits.
            NativeMethods.SystemParametersInfo(NativeMethods.SPI_SETCURSORS, 0, IntPtr.Zero, 0);
        }
    }
}
