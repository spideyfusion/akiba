namespace Akiba.Core
{
    using Akiba.Core.Exceptions;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    internal class AkibaInstaller
    {
        public FileVersionInfo Utility { get; }

        public enum InstallStatus
        {
            Installed,
            UpgradeRequested,
            Launched,
        }

        private readonly string processName;

        public AkibaInstaller()
        {
            this.processName = Process.GetCurrentProcess().MainModule.FileName;

            if (File.Exists(Utilities.ConfigExecutableName))
            {
                this.Utility = FileVersionInfo.GetVersionInfo(Utilities.ConfigExecutableName);
            }
        }

        public InstallStatus Install()
        {
            if (!File.Exists(Utilities.GameExecutableName))
            {
                throw new BootstrapException(Properties.Resources.MessageGameLocation);
            }

            if (Path.GetFileName(this.processName).Equals(Utilities.ConfigExecutableName))
            {
                if (!File.Exists(Utilities.BackupConfigExecutableName))
                {
                    throw new BootstrapException(Properties.Resources.MessageBackupMissing);
                }

                return InstallStatus.Launched;
            }

            if (this.Utility == null)
            {
                throw new BootstrapException(Properties.Resources.MessageConfigUtilityMissing);
            }

            if (this.Utility.ProductName.Equals(Application.ProductName))
            {
                return InstallStatus.UpgradeRequested;
            }

            if (File.Exists(Utilities.BackupConfigExecutableName))
            {
                // The config utility was most likely updated.
                File.Delete(Utilities.BackupConfigExecutableName);
            }

            // Even though we're replacing the config utlity, we don't want to get rid of it.
            File.Move(Utilities.ConfigExecutableName, Utilities.BackupConfigExecutableName);

            // We'll rename our utility to match the game's config executable name.
            File.Move(this.processName, Utilities.ConfigExecutableName);

            return InstallStatus.Installed;
        }

        public bool Upgrade()
        {
            if (this.Utility == null)
            {
                return false;
            }

            // Send the old utility version to oblivion...
            File.Delete(Utilities.ConfigExecutableName);

            // And let the new one take its place...
            File.Move(this.processName, Utilities.ConfigExecutableName);

            return true;
        }
    }
}
