using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Guncon3Console.TetherScript
{
    static class AbsMouseFeeder
    {
        private static readonly HIDController HID = new HIDController();

        public static readonly Dictionary<GunButton, MouseButton> Mapping = new Dictionary<GunButton, MouseButton>();
        public static bool Force4by3 = false;
        private static byte btns = 0;

        public static void Connect()
        {
            //create the HIDController 
            HID.OnLog += Log;
            HID.VendorID = (ushort)DriversConst.TTC_VENDORID;                //the Tetherscript vendorid
            HID.ProductID = (ushort)DriversConst.TTC_PRODUCTID_MOUSEABS;     //the Tetherscript Virtual Mouse Absolute Driver productid
            HID.Connect();
            if (!HID.Connected)
                throw new Exception("Coud not connect to TetherScript's AbsMouse");
        }

        public static void Disconnect()
        {
            HID.Disconnect();
            HID.OnLog -= Log;
        }

        public static void Log(object s, LogArgs e)
        {
            Console.WriteLine("Mouse " + e.Msg);
        }

        public static void Send_Data_To_MouseAbs(ushort x, ushort y)
        {
            SetFeatureMouseAbs MouseAbsData = new SetFeatureMouseAbs();
            MouseAbsData.ReportID = 1;
            MouseAbsData.CommandCode = 2;
            //byte btns = 0;
            //if (left) { btns = 1; };
            //if (right) { btns = (byte)(btns | (1 << 1)); }
            //if (middle) { btns = (byte)(btns | (1 << 2)); }
            MouseAbsData.Buttons = btns;  //button states are represented by the 3 least significant bits

            MouseAbsData.X = x;
            MouseAbsData.Y = y;

            //convert struct to buffer
            byte[] buf = getBytesSFJ(MouseAbsData, Marshal.SizeOf(MouseAbsData));
            //send filled buffer to driver
            HID.SendData(buf, (uint)Marshal.SizeOf(MouseAbsData));

            //if (btns != 0)
            //    System.Threading.Thread.Sleep(0);
        }

        //for converting a struct to byte array
        public static byte[] getBytesSFJ(SetFeatureMouseAbs sfj, int size)
        {
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(sfj, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        internal static void Feed()
        {
            short absX = 0;
            short absY = 0;

            if (GunState.IsInsideScreen)
            {
                absX = GunState.ABS_X;//(short)Helper.ConvertRange(0, 32767, 0, 32767, GunState.ABS_X);
                absY = GunState.ABS_Y;//(short)Helper.ConvertRange(0, 32767, 0, 32767, GunState.ABS_Y);

                //set for 4:3 ratio inside widescreen resolution (for MAME)
                if (Force4by3)
                    absX = (short)Helper.ConvertRange(4096, 28671, 0, 32767, absX);
            }

            btns = 0;

            foreach (var item in Mapping)
            {
                if (GunState.BtnState[item.Key])
                {
                    if (item.Value == MouseButton.Left)
                        btns = 1;
                    else if (item.Value == MouseButton.Right)
                        btns = (byte)(btns | (1 << 1));
                    else if (item.Value == MouseButton.Middle)
                        btns = (byte)(btns | (1 << 2));
                }
            }

            Send_Data_To_MouseAbs((ushort)absX, (ushort)absY);//GunState.BTN_TRIGGER, GunState.C1, GunState.C2
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SetFeatureMouseAbs
    {
        public byte ReportID;
        public byte CommandCode;
        public byte Buttons;
        public ushort X;
        public ushort Y;
    }
}
