using Akiba.Core.Exceptions;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Akiba.Core
{
    class AkibaInstaller
    {
        private string ProcessName;
        private FileVersionInfo UtilityVersion;

        public FileVersionInfo OldUtility
        {
            get
            {
                return UtilityVersion;
            }
        }

        public enum InstallStatus
        {
            Installed,
            UpgradeRequested,
            Launched,
        }

        public AkibaInstaller()
        {
            this.ProcessName = Process.GetCurrentProcess().MainModule.FileName;

            if (File.Exists(Utilities.ConfigExecutableName))
            {
                this.UtilityVersion = FileVersionInfo.GetVersionInfo(Utilities.ConfigExecutableName);
            }
        }

        public InstallStatus Install()
        {
            if (!File.Exists(Utilities.GameExecutableName))
            {
                throw new BootstrapException(Properties.Resources.MessageGameLocation);
            }

            if (Path.GetFileName(this.ProcessName).Equals(Utilities.ConfigExecutableName))
            {
                if (!File.Exists(Utilities.BackupConfigExecutableName))
                {
                    throw new BootstrapException(Properties.Resources.MessageBackupMissing);
                }

                return InstallStatus.Launched;
            }

            if (this.UtilityVersion == null)
            {
                throw new BootstrapException(Properties.Resources.MessageConfigUtilityMissing);
            }

            if (this.IsUtilityOurOwn())
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
            File.Move(this.ProcessName, Utilities.ConfigExecutableName);

            return InstallStatus.Installed;
        }

        public bool Upgrade()
        {
            if (this.UtilityVersion == null)
            {
                return false;
            }

            // Send the old utility version to oblivion...
            File.Delete(Utilities.ConfigExecutableName);

            // And let the new one take its place...
            File.Move(this.ProcessName, Utilities.ConfigExecutableName);

            return true;
        }

        private bool IsUtilityOurOwn()
        {
            if (!this.UtilityVersion.ProductName.Equals(Application.ProductName))
            {
                return false;
            }

            return true;
        }
    }
}
