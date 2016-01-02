using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Akiba
{
    static class Utilities
    {
        public static string GameExecutableName = "AkibaUU.exe";
        public static string ConfigExecutableName = "AkibaUU_Config.exe";
        public static string BackupConfigExecutableName = "AkibaUU_Config.Original.exe";
        public static string ConfigTriggerSwitch = "RediationNext_Config";

        public static bool AlterGameFramerate()
        {
            var gameProcess = Process.GetProcessesByName(
                Path.GetFileNameWithoutExtension(GameExecutableName)
            ).FirstOrDefault();

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

            return 0x00;
        }
    }
}
