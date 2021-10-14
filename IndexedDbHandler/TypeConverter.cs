using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSWrapper
{
    internal static class TypeConverter
    {
        public static unsafe string GetStringFromBytes(byte[] bin)
        {
            fixed (void* ptr = bin)
            {
                return new string(new ReadOnlySpan<char>(ptr, bin.Length >> 1));
            }
        }

        public static unsafe byte[] StringToBytes(string str)
        {
            int len = str.Length << 1;
            byte[] bin = new byte[len];
            fixed (void* arrPtr = bin)
            {
                fixed (void* strPtr = str)
                {
                    Buffer.MemoryCopy(strPtr, arrPtr, len, len);
                }
            }
            return bin;
        }
    }
}
