using GitHubNewsGenerator.Native;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace GitHubNewsGenerator
{
    public static class Clipboard
    {
        public static void SetText(ReadOnlySpan<char> value)
        {
            var memoryLength = (value.Length + 1) * sizeof(char);

            // SetClipboardData requires GMEM_MOVEABLE.
            // https://docs.microsoft.com/en-us/windows/desktop/api/Winuser/nf-winuser-setclipboarddata#parameters
            using (var memory = Kernel32.GlobalAlloc(Kernel32.GMEM.MOVEABLE, (UIntPtr)memoryLength))
            {
                unsafe
                {
                    var pointer = Kernel32.GlobalLock(memory);
                    if (pointer is null) throw new Win32Exception();
                    try
                    {
                        fixed (char* valuePointer = value)
                            Buffer.MemoryCopy(valuePointer, pointer, memoryLength, memoryLength);

                        *((char*)pointer + value.Length) = '\0';
                    }
                    finally
                    {
                        if (!Kernel32.GlobalUnlock(memory))
                        {
                            var error = Marshal.GetLastWin32Error();
                            if (error != 0) throw new Win32Exception(error);
                        }
                    }
                }

                if (!User32.OpenClipboard(hWndNewOwner: IntPtr.Zero))
                    throw new Win32Exception();
                try
                {
                    if (User32.SetClipboardData(User32.CF.UNICODETEXT, memory) == IntPtr.Zero)
                        throw new Win32Exception();

                    // If SetClipboardData succeeds, we have transferred ownership of the memory.
                    // https://docs.microsoft.com/en-us/windows/desktop/api/Winuser/nf-winuser-setclipboarddata#parameters
                    memory.SetHandleAsInvalid();
                }
                finally
                {
                    if (!User32.CloseClipboard())
                        throw new Win32Exception();
                }
            }
        }
    }
}
