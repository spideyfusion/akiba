using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Akiba.Core
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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool repaint);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr handle, out Rect rect);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr handle, int index, int newLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam);

        // GetWindowLong API styles
        public const int GWL_STYLE = -16;

        // Window styles
        public const int WS_VISIBLE = 0x10000000;

        // Window messages
        public const int WM_SYSCOMMAND = 0x0112;

        // System commands
        public const int SC_RESTORE = 0xF120;

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

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static void WriteToProcessMemory(Process process, int address, long v)
        {
            var processHandle = OpenProcess(ProcessAccessFlags.All, false, (int)process.Id);
            var value = new byte[] { (byte)v };

            WriteProcessMemory(processHandle, new IntPtr(address), value, (UIntPtr)value.LongLength, out int bytesWritten);

            CloseHandle(processHandle);
        }
    }
}
