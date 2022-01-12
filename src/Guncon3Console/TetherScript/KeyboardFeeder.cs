using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Guncon3Console.TetherScript
{
    static class KeyboardFeeder
    {
        private static readonly HIDController HID = new HIDController();
        private static readonly uint FTimeout = 5000;  //approx five seconds

        public static readonly Dictionary<GunButton, byte> Mapping = new Dictionary<GunButton, byte>();
        private static readonly byte[] keysToSend = new byte[6];

        public static void Connect()
        {
            HID.OnLog += Log;
            HID.VendorID = (ushort)DriversConst.TTC_VENDORID;                //the Tetherscript vendorid
            HID.ProductID = (ushort)DriversConst.TTC_PRODUCTID_KEYBOARD;     //the Tetherscript Virtual Keyboard Driver productid
            HID.Connect();
            if (!HID.Connected)
                throw new Exception("Coud not connect to TetherScript's Keyboard.");
        }

        public static void Disconnect()
        {
            HID.Disconnect();
            HID.OnLog -= Log;
        }

        public static void Log(object s, LogArgs e)
        {
            Console.WriteLine("Keyboard " + e.Msg);
        }

        public static void Send(byte Modifier, byte Padding, byte Key0, byte Key1, byte Key2, byte Key3, byte Key4, byte Key5)
        {
            SetFeatureKeyboard KeyboardData = new SetFeatureKeyboard();
            KeyboardData.ReportID = 1;
            KeyboardData.CommandCode = 2;
            KeyboardData.Timeout = FTimeout / 5; //5 because we count in blocks of 5 in the driver
            KeyboardData.Modifier = Modifier;
            //padding should always be zero.
            KeyboardData.Padding = Padding;
            KeyboardData.Key0 = Key0;
            KeyboardData.Key1 = Key1;
            KeyboardData.Key2 = Key2;
            KeyboardData.Key3 = Key3;
            KeyboardData.Key4 = Key4;
            KeyboardData.Key5 = Key5;
            //convert struct to buffer
            byte[] buf = getBytesSFJ(KeyboardData, Marshal.SizeOf(KeyboardData));
            //send filled buffer to driver
            HID.SendData(buf, (uint)Marshal.SizeOf(KeyboardData));
        }

        public static void Ping()
        {
            SetFeatureKeyboard KeyboardData = new SetFeatureKeyboard();
            KeyboardData.ReportID = 1;
            KeyboardData.CommandCode = 3;
            //the timeout is how long the driver will wait (milliseconds) without receiving a ping before resetting itself
            //we'll be pinging every 200ms, and loss of ping will cause driver reset in FTimeout.  
            //No more stuck keys requiring reboot to clear.
            //the following fields are not used by the driver for a ping, but we'll zero them anyways
            KeyboardData.Timeout = FTimeout / 5; //50 because we count in blocks of 50 in the driver;
            KeyboardData.Modifier = 0;
            KeyboardData.Padding = 0;
            KeyboardData.Key0 = 0;
            KeyboardData.Key1 = 0;
            KeyboardData.Key2 = 0;
            KeyboardData.Key3 = 0;
            KeyboardData.Key4 = 0;
            KeyboardData.Key5 = 0;
            //convert struct to buffer
            byte[] buf = getBytesSFJ(KeyboardData, Marshal.SizeOf(KeyboardData));
            //send filled buffer to driver
            HID.SendData(buf, (uint)Marshal.SizeOf(KeyboardData));
        }

        //for converting a struct to byte array
        public static byte[] getBytesSFJ(SetFeatureKeyboard sfj, int size)
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
            Ping();

            if (!Mapping.Any())
                return;

            //reset key data
            for (int i = 0; i < 6; i++)
                keysToSend[i] = 0;
            
            int index = 0;
            foreach (var item in Mapping)
            {
                if (index == 6)
                    break;
                if (GunState.BtnState[item.Key])
                {
                    keysToSend[index] = item.Value;
                    index++;
                }
            }

            Send(0, 0, keysToSend[0], keysToSend[1], keysToSend[2], keysToSend[3], keysToSend[4], keysToSend[5]);
            Thread.Sleep(1);
            Send(0, 0, 0, 0, 0, 0, 0, 0);//reset pressed keys
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SetFeatureKeyboard
    {
        public byte ReportID;
        public byte CommandCode;
        public uint Timeout;
        public byte Modifier;
        public byte Padding;
        public byte Key0;
        public byte Key1;
        public byte Key2;
        public byte Key3;
        public byte Key4;
        public byte Key5;
    }
}
