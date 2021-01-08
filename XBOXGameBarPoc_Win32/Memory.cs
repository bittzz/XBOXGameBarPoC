using System;
using System.Runtime.InteropServices;

namespace XBOXGameBarPoC_Win32
{
    static class Memory
    {
        private static IntPtr pHandle;

        public static void SetHandle(IntPtr handle)
        {
            pHandle = handle;
        }

        public static T Read<T>(IntPtr address) where T : struct
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
            Native.ReadProcessMemory(pHandle, address, buffer, buffer.Length, out _);

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T retVal = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return retVal;
        }

        public static T[] ReadArray<T>(IntPtr address, int count) where T : struct
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T)) * count];
            Native.ReadProcessMemory(pHandle, address, buffer, buffer.Length, out _);

            T[] retVal = new T[count];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            for (int i = 0; i < count; i++)
            {
                retVal[i] = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject() + i * Marshal.SizeOf(typeof(T)), typeof(T));
            }
            handle.Free();
            return retVal;
        }
    }

    class Native
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
    }
}
