namespace Akiba.Core
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    internal static class Utilities
    {
        public const string GameExecutableName = "AkibaUU.exe";
        public const string GameIniName = "AkibaUU.ini";
        public const string ConfigExecutableName = "AkibaUU_Config.exe";
        public const string BackupConfigExecutableName = "AkibaUU_Config.Original.exe";
        public const string ConfigTriggerSwitch = "RediationNext_Config";
        public const string MonitorTriggerSwitch = "GameMonitor";

        public static Process GetGameProcess() => Process.GetProcessesByName(Path.GetFileNameWithoutExtension(GameExecutableName)).FirstOrDefault();
    }
}
