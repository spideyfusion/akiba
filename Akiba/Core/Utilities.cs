using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Akiba.Core
{
    static class Utilities
    {
        public const uint SteamApplicationId = 333980;

        public const string GameExecutableName = "AkibaUU.exe";
        public const string ConfigExecutableName = "AkibaUU_Config.exe";
        public const string BackupConfigExecutableName = "AkibaUU_Config.Original.exe";
        public const string ConfigTriggerSwitch = "RediationNext_Config";
        public const string MonitorTriggerSwitch = "GameMonitor";

        public static bool AlterGameFramerate()
        {
            Process gameProcess = GetGameProcess();

            if (gameProcess == null)
            {
                return false;
            }

            int memoryAddress = GetFramerateMemoryAddress();

            if (memoryAddress == 0x00)
            {
                return false;
            }

            NativeMethods.WriteToProcessMemory(gameProcess, memoryAddress, Program.Config.FramesPerSecond);

            return true;
        }

        public static Process GetGameProcess()
        {
            return Process.GetProcessesByName(
                Path.GetFileNameWithoutExtension(GameExecutableName)
            ).FirstOrDefault();
        }

        public static void LaunchSteamGame(uint applicationId)
        {
            Process.Start(string.Format("steam://run/{0}", applicationId));
        }

        private static int GetFramerateMemoryAddress()
        {
            Version systemVersion = Environment.OSVersion.Version;

            if (systemVersion.Major == 10)
            {
                // Windows 10
                return 0x0019FDFC;
            }
            else if (systemVersion.Major == 6 && systemVersion.Minor > 1)
            {
                // Windows 8/8.1
                return 0x0018FDFC;
            }
            else if (systemVersion.Major == 6 && systemVersion.Minor == 1)
            {
                // Windows 7
                return 0x0018FE04;
            }
            else if (systemVersion.Major == 6 && systemVersion.Minor == 0)
            {
                // Windows Vista
                return 0x0017FE04;
            }

            return 0x00;
        }
    }
}
