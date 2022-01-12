using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GunconUSB
{
    static class GammaManager
    {

        [DllImport("gdi32.dll")]
        private unsafe static extern bool SetDeviceGammaRamp(Int32 hdc, void* ramp);

        [DllImport("gdi32.dll")]
        private unsafe static extern bool SetDeviceGammaRamp(Int32 hdc, ref RAMP lpRamp);

        [DllImport("gdi32.dll")]
        private unsafe static extern bool GetDeviceGammaRamp(Int32 hdc, ref RAMP lpRamp);


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Blue;
        }

        private static bool initialized = false;
        private static Int32 hdc;

        private static RAMP originalRamp = new RAMP();

        private static void InitializeClass()
        {
            if (initialized)
                return;
            hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc().ToInt32();

            GetDeviceGammaRamp(hdc, ref originalRamp);

            initialized = true;
        }

        public static unsafe bool GetBrightness()
        {
            InitializeClass();

            RAMP r = new RAMP();
            GetDeviceGammaRamp(hdc, ref r);
            var aaaa = Color.FromArgb(r.Red[1], r.Green[1], r.Blue[1]);
            return true;



            //if (brightness > 255)
            //    brightness = 255;

            //if (brightness < 0)
            //    brightness = 0;

            short* gArray = stackalloc short[3 * 256];
            //short* idx = gArray;

            //for (int j = 0; j < 3; j++)
            //{
            //    for (int i = 0; i < 256; i++)
            //    {
            //        int arrayVal = i * (brightness + 128);

            //        if (arrayVal > 65535)
            //            arrayVal = 65535;

            //        *idx = (short)arrayVal;
            //        idx++;
            //    }
            //}

            //For some reason, this always returns false?
            //bool retVal = GetDeviceGammaRamp(hdc, gArray);

            //Memory allocated through stackalloc is automatically free'd
            //by the CLR.

            //return retVal;
        }

        public static unsafe bool RestoreBrightness()
        {
            if (!initialized)
                return false;

            bool retVal = SetDeviceGammaRamp(hdc, ref originalRamp);
            return retVal;
        }

        public static unsafe bool SetBrightness(short brightness)
        {
            InitializeClass();

            if (brightness > 255)
                brightness = 255;

            if (brightness < 0)
                brightness = 0;

            short* gArray = stackalloc short[3 * 256];
            short* idx = gArray;

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 256; i++)
                {
                    int arrayVal = i * (brightness + 128);

                    if (arrayVal > 65535)
                        arrayVal = 65535;

                    *idx = (short)arrayVal;
                    idx++;
                }
            }

            bool retVal = SetDeviceGammaRamp(hdc, gArray);

            return retVal;
        }

    }
}
