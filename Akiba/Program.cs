using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

            if (!InitialBootstrap())
            {
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

            if (!InvokedByAkiba(args))
            {
                Process gameUtility = Process.Start(new ProcessStartInfo {
                    FileName = Utilities.BackupConfigExecutableName,
                    Arguments = Utilities.ConfigTriggerSwitch
                });

                gameUtility.WaitForExit();

                if (gameUtility.ExitCode == 0)
                {
                    Process.Start(Utilities.GameExecutableName);
                }

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

                // Even we're replacing the config utlity, we don't want to get rid of it.
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
                return Configuration.LoadFromFile();
            }
            catch
            {
                return null;
            }
        }

        private static bool InvokedByAkiba(string[] args)
        {
            return args.DefaultIfEmpty(string.Empty).First().Equals(Utilities.ConfigTriggerSwitch);
        }
    }
}
