using System;
using System.Runtime.InteropServices;

namespace GitHubNewsGenerator.Native
{
    internal static class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetClipboardData(CF uFormat, Kernel32.SafeGlobalMemoryHandle hMem);

        public enum CF : uint
        {
            UNICODETEXT = 13
        }
    }
}
