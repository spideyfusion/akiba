using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Akiba
{
    static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr process, IntPtr baseAddress, byte[] buffer, UIntPtr size, out int numberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr process);

        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(ExecutionStateFlags flags);

        [Flags]
        public enum ExecutionStateFlags : uint
        {
            AwayModeRequired = 0x00000040,
            Continuous = 0x80000000,
            DisplayRequired = 0x00000002,
            SystemRequired = 0x00000001,
            UserPresent = 0x00000004,
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000,
        }

        public static void WriteToProcessMemory(Process process, int address, long v)
        {
            var processHandle = OpenProcess(ProcessAccessFlags.All, false, (int)process.Id);
            var value = new byte[] { (byte)v };

            int bytesWritten = 0;
            WriteProcessMemory(processHandle, new IntPtr(address), value, (UIntPtr)value.LongLength, out bytesWritten);

            CloseHandle(processHandle);
        }
    }
}
