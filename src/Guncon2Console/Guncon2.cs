using MadWizard.WinUSBNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guncon2Console
{
    internal static class Guncon2
    {
        private const int pid = 362;
        private const int vid = 2970;
        private static USBDevice device = null;
        private static readonly Guid deviceguid = new Guid("{A5DCBF10-6530-11D2-901F-00C04FB951ED}");

        internal static void Connect()
        {
            try
            {
                Console.WriteLine("Guncon2 connecting...");
                var deviceInfo = USBDevice.GetDevices(deviceguid).Where(x => x.PID == pid && x.VID == vid).FirstOrDefault();
                device = new USBDevice(deviceInfo);

                //change guncon mode
                byte[] command = new byte[] { 0, 0, 0, 0, 0, 1 };
                device.ControlOut(0x21, 0x09, 0x200, 0, command);

                Console.WriteLine("Guncon2 connected.");
            }
            catch (Exception)
            {
                Disconnect();
                throw;
            }
        }

        internal static void Disconnect()
        {
            try
            {
                if (device != null)
                {
                    device.Dispose();
                    device = null;
                }
            }
            catch (Exception)
            {
                
            }
        }

        internal static void Read()
        {
            var iface = device.Interfaces[0];
            //iface.OutPipe.Write(key);

            byte[] data = new byte[6];

            if (iface.InPipe.Read(data) != 6)
                throw new Exception("Invalid Guncon read length");

            var decoded = data;// guncondecode(data);

            //var state = new GunState
            //{
            //    ABS_RY = decoded[0],//B
            //    ABS_RX = decoded[1],
            //    ABS_HAT0Y = decoded[2],//A
            //    ABS_HAT0X = decoded[3],
            //    BTN_TRIGGER = (decoded[11] & 0x20) != 0,
            //    A1 = (decoded[12] & 0x04) != 0,
            //    A2 = (decoded[12] & 0x02) != 0,
            //    B1 = (decoded[11] & 0x04) != 0,
            //    B2 = (decoded[11] & 0x02) != 0,
            //    C1 = (decoded[11] & 0x80) != 0,
            //    C2 = (decoded[12] & 0x08) != 0,
            //    Z = (short)(decoded[4] * 256 + decoded[5]),
            //    ABS_Y = (short)(decoded[6] * 256 + decoded[7]),
            //    ABS_X = (short)(decoded[8] * 256 + decoded[9]),
            //    A_STICK_BUTTON = (decoded[10] & 0x80) != 0,
            //    B_STICK_BUTTON = (decoded[10] & 0x40) != 0,

            //    INDICATOR1 = (decoded[11] & 0x10) != 0,
            //    INDICATOR2 = (decoded[11] & 0x08) != 0,
            //};

            GunState.BtnState[GunButton.Trigger] = (data[1] & 0x20) == 0;
            GunState.BtnState[GunButton.A1] = (data[0] & 0x08) == 0;
            GunState.BtnState[GunButton.A2] = (data[0] & 0x04) == 0;
            GunState.BtnState[GunButton.B1] = (data[0] & 0x02) == 0;
            GunState.BtnState[GunButton.B2] = (data[1] & 0x80) == 0;
            GunState.BtnState[GunButton.C1] = (data[1] & 0x40) == 0;
            //GunState.BtnState[GunButton.C2] = (decoded[12] & 0x08) != 0;
            //GunState.BtnState[GunButton.AClick] = (decoded[10] & 0x80) != 0;
            //GunState.BtnState[GunButton.BClick] = (decoded[10] & 0x40) != 0;


            uint gunX;
            gunX = data[3];
            gunX <<= 8;
            gunX |= data[2];


            uint gunY;
            gunY = data[5];
            gunY <<= 8;
            gunY |= data[4];




            //GunState.ABS_RY = decoded[0];//B
            //GunState.ABS_RX = decoded[1];
            //GunState.ABS_HAT0Y = decoded[2];//A
            //GunState.ABS_HAT0X = decoded[3];
            //GunState.BTN_TRIGGER = (decoded[11] & 0x20) != 0;
            //GunState.A1 = (decoded[12] & 0x04) != 0;
            //GunState.A2 = (decoded[12] & 0x02) != 0;
            //GunState.B1 = (decoded[11] & 0x04) != 0;
            //GunState.B2 = (decoded[11] & 0x02) != 0;
            //GunState.C1 = (decoded[11] & 0x80) != 0;
            //GunState.C2 = (decoded[12] & 0x08) != 0;
            //GunState.Z = (short)(decoded[4] * 256 + decoded[5]);
            GunState.ABS_Y = (short)gunY;//(short)(decoded[6] * 256 + decoded[7]);
            GunState.ABS_X = (short)gunX;//(short)(decoded[8] * 256 + decoded[9]);
            //GunState.A_STICK_BUTTON = (decoded[10] & 0x80) != 0;
            //GunState.B_STICK_BUTTON = (decoded[10] & 0x40) != 0;
            //GunState.INDICATOR1 = (decoded[11] & 0x10) != 0;
            //GunState.INDICATOR2 = (decoded[11] & 0x08) != 0;



            if (GunState.ABS_X > 10)
                GunState.MinX = Math.Min(GunState.MinX, GunState.ABS_X);
            if (GunState.ABS_Y > 5)
                GunState.MinY = Math.Min(GunState.MinY, GunState.ABS_Y);


            GunState.MaxX = Math.Max(GunState.MaxX, GunState.ABS_X);
            GunState.MaxY = Math.Max(GunState.MaxY, GunState.ABS_Y);


            if (Calibration.k_coefs_seted)
            {
                short x = GunState.ABS_X;
                short y = GunState.ABS_Y;
                Calibration.Do_Calibration(ref x, ref y);
                GunState.ABS_X = x;
                GunState.ABS_Y = y;
            }

            //return state;

        }

        

    }

    public enum GunButton
    {
        Trigger,
        A1,
        A2,
        B1,
        B2,
        C1,
        C2,
        AClick,
        BClick,
    }
}
