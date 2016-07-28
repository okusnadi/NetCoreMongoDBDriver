using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace MongoDB.Driver.Core
{
    public class MarshalExtentsions
    {

        public static string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = SecureStringMarshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static IntPtr SecureStringToBSTR(SecureString value)
        {
            var str = SecureStringToString(value);
            try
            {
                return Marshal.StringToBSTR(str);
            }
            finally
            {

            }
        }


    }
}
