namespace Akiba.Core
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal static class Utilities
    {
        private const uint SteamApplicationIdFull = 333980;
        private const uint SteamApplicationIdDemo = 375980;

        public const string GameExecutableName = "AkibaUU.exe";
        public const string GameIniName = "AkibaUU.ini";
        public const string ConfigExecutableName = "AkibaUU_Config.exe";
        public const string BackupConfigExecutableName = "AkibaUU_Config.Original.exe";
        public const string ConfigTriggerSwitch = "RediationNext_Config";
        public const string MonitorTriggerSwitch = "GameMonitor";

        public static Process GetGameProcess() => Process.GetProcessesByName(Path.GetFileNameWithoutExtension(GameExecutableName)).FirstOrDefault();

        public static void LaunchSteamGame(uint applicationId) => _ = Process.Start(string.Format("steam://run/{0}", applicationId));

        public static uint GetSteamApplicationId(Assembly assembly)
        {
            if (Path.GetDirectoryName(assembly.Location).ToLower().EndsWith("demo"))
            {
                return SteamApplicationIdDemo;
            }

            return SteamApplicationIdFull;
        }
    }
}
