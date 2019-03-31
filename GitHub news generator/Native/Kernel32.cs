using System;
using System.Runtime.InteropServices;

namespace GitHubNewsGenerator.Native
{
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeGlobalMemoryHandle GlobalAlloc(GMEM uFlags, UIntPtr dwBytes);

        public enum GMEM : uint
        {
            MOVEABLE = 2
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe void* GlobalLock(SafeGlobalMemoryHandle hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GlobalUnlock(SafeGlobalMemoryHandle hMem);

        public sealed class SafeGlobalMemoryHandle : SafeHandle
        {
            private SafeGlobalMemoryHandle()
                : base(invalidHandleValue: IntPtr.Zero, ownsHandle: true)
            {
            }

            public override bool IsInvalid => handle == IntPtr.Zero;

            protected override bool ReleaseHandle() => GlobalFree(handle) == IntPtr.Zero;

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GlobalFree(IntPtr hMem);
        }
    }
}
