using MadWizard.WinUSBNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guncon3Console
{
    internal static class Guncon3
    {
        private const int pid = 2048;
        private const int vid = 2970;
        private static USBDevice device = null;
        private static readonly Guid deviceguid = new Guid("{A5DCBF10-6530-11D2-901F-00C04FB951ED}");

        private static readonly byte[] key = new byte[] { 0x01, 0x12, 0x6f, 0x32, 0x24, 0x60, 0x17, 0x21 };

        private static readonly byte[] KEY_TABLE = new byte[]{
            0x75, 0xC3, 0x10, 0x31, 0xB5, 0xD3, 0x69, 0x84, 0x89, 0xBA, 0xD6, 0x89, 0xBD, 0x70, 0x19, 0x8E, 0x58, 0xA8,
            0x3D, 0x9B, 0x5D, 0xF0, 0x49, 0xE8, 0xAD, 0x9D, 0x7A, 0xD, 0x7E, 0x24, 0xDA, 0xFC, 0xD, 0x14, 0xC5, 0x23,
            0x91, 0x11, 0xF5, 0xC0, 0x4B, 0xCD, 0x44, 0x1C, 0xC5, 0x21, 0xDF, 0x61, 0x54, 0xED, 0xA2, 0x81, 0xB7, 0xE5,
            0x74, 0x94, 0xB0, 0x47, 0xEE, 0xF1, 0xA5, 0xBB, 0x21, 0xC8, 0x91, 0xFD, 0x4C, 0x8B, 0x20, 0xC1, 0x7C, 9, 0x58,
            0x14, 0xF6, 0, 0x52, 0x55, 0xBF, 0x41, 0x75, 0xC0, 0x13, 0x30, 0xB5, 0xD0, 0x69, 0x85, 0x89, 0xBB, 0xD6, 0x88,
            0xBC, 0x73, 0x18, 0x8D, 0x58, 0xAB, 0x3D, 0x98, 0x5C, 0xF2, 0x48, 0xE9, 0xAC, 0x9F, 0x7A, 0xC, 0x7C, 0x25, 0xD8,
            0xFF, 0xDC, 0x7D, 8, 0xDB, 0xBC, 0x18, 0x8C, 0x1D, 0xD6, 0x3C, 0x35, 0xE1, 0x2C, 0x14, 0x8E, 0x64, 0x83, 0x39,
            0xB0, 0xE4, 0x4E, 0xF7, 0x51, 0x7B, 0xA8, 0x13, 0xAC, 0xE9, 0x43, 0xC0, 8, 0x25, 0xE, 0x15, 0xC4, 0x20, 0x93,
            0x13, 0xF5, 0xC3, 0x48, 0xCC, 0x47, 0x1C, 0xC5, 0x20, 0xDE, 0x60, 0x55, 0xEE, 0xA0, 0x40, 0xB4, 0xE7, 0x74,
            0x95, 0xB0, 0x46, 0xEC, 0xF0, 0xA5, 0xB8, 0x23, 0xC8, 4, 6, 0xFC, 0x28, 0xCB, 0xF8, 0x17, 0x2C, 0x25, 0x1C,
            0xCB, 0x18, 0xE3, 0x6C, 0x80, 0x85, 0xDD, 0x7E, 9, 0xD9, 0xBC, 0x19, 0x8F, 0x1D, 0xD4, 0x3D, 0x37, 0xE1, 0x2F,
            0x15, 0x8D, 0x64, 6, 4, 0xFD, 0x29, 0xCF, 0xFA, 0x14, 0x2E, 0x25, 0x1F, 0xC9, 0x18, 0xE3, 0x6D, 0x81, 0x84,
            0x80, 0x3B, 0xB1, 0xE5, 0x4D, 0xF7, 0x51, 0x78, 0xA9, 0x13, 0xAD, 0xE9, 0x80, 0xC1, 0xB, 0x25, 0x93, 0xFC,
            0x4D, 0x89, 0x23, 0xC2, 0x7C, 0xB, 0x59, 0x15, 0xF6, 1, 0x50, 0x55, 0xBF, 0x81, 0x75, 0xC3, 0x10, 0x31, 0xB5,
            0xD3, 0x69, 0x84, 0x89, 0xBA, 0xD6, 0x89, 0xBD, 0x70, 0x19, 0x8E, 0x58, 0xA8, 0x3D, 0x9B, 0x5D, 0xF0, 0x49,
            0xE8, 0xAD, 0x9D, 0x7A, 0xD, 0x7E, 0x24, 0xDA, 0xFC, 0xD, 0x14, 0xC5, 0x23, 0x91, 0x11, 0xF5, 0xC0, 0x4B, 0xCD,
            0x44, 0x1C, 0xC5, 0x21, 0xDF, 0x61, 0x54, 0xED, 0xA2, 0x81, 0xB7, 0xE5, 0x74, 0x94, 0xB0, 0x47, 0xEE, 0xF1,
            0xA5, 0xBB, 0x21, 0xC8
        };


        internal static void Connect()
        {
            try
            {
                Console.WriteLine("Guncon3 connecting...");
                var deviceInfo = USBDevice.GetDevices(deviceguid).Where(x => x.PID == pid && x.VID == vid).FirstOrDefault();
                if (deviceInfo == null)
                    throw new Exception("Guncon3 device not found");

                device = new USBDevice(deviceInfo);
                Console.WriteLine("Guncon3 connected.");
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
            iface.OutPipe.Write(key);

            byte[] data = new byte[15];

            if (iface.InPipe.Read(data) != 15)
                throw new Exception("Invalid Guncon read length");

            var decoded = guncondecode(data);

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

            GunState.BtnState[GunButton.Trigger] = (decoded[11] & 0x20) != 0;
            GunState.BtnState[GunButton.A1] = (decoded[12] & 0x04) != 0;
            GunState.BtnState[GunButton.A2] = (decoded[12] & 0x02) != 0;
            GunState.BtnState[GunButton.B1] = (decoded[11] & 0x04) != 0;
            GunState.BtnState[GunButton.B2] = (decoded[11] & 0x02) != 0;
            GunState.BtnState[GunButton.C1] = (decoded[11] & 0x80) != 0;
            GunState.BtnState[GunButton.C2] = (decoded[12] & 0x08) != 0;
            GunState.BtnState[GunButton.AClick] = (decoded[10] & 0x80) != 0;
            GunState.BtnState[GunButton.BClick] = (decoded[10] & 0x40) != 0;




            GunState.ABS_RY = decoded[0];//B
            GunState.ABS_RX = decoded[1];
            GunState.ABS_HAT0Y = decoded[2];//A
            GunState.ABS_HAT0X = decoded[3];
            //GunState.BTN_TRIGGER = (decoded[11] & 0x20) != 0;
            //GunState.A1 = (decoded[12] & 0x04) != 0;
            //GunState.A2 = (decoded[12] & 0x02) != 0;
            //GunState.B1 = (decoded[11] & 0x04) != 0;
            //GunState.B2 = (decoded[11] & 0x02) != 0;
            //GunState.C1 = (decoded[11] & 0x80) != 0;
            //GunState.C2 = (decoded[12] & 0x08) != 0;
            GunState.Z = (short)(decoded[4] * 256 + decoded[5]);
            GunState.ABS_Y = (short)(decoded[6] * 256 + decoded[7]);
            GunState.ABS_X = (short)(decoded[8] * 256 + decoded[9]);
            //GunState.A_STICK_BUTTON = (decoded[10] & 0x80) != 0;
            //GunState.B_STICK_BUTTON = (decoded[10] & 0x40) != 0;
            GunState.INDICATOR1 = (decoded[11] & 0x10) != 0;
            GunState.INDICATOR2 = (decoded[11] & 0x08) != 0;

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

        private static List<byte> guncondecode(byte[] data2)
        {
            //byte[] ret;
            var ret = new List<byte>();
            long x, y, key_index;
            long bkey, keyr, _byte;
            long a_sum, b_sum;
            long key_offset;
            byte[] data = new byte[15];

            if (data2.Length != 15)
                return ret;

            for (int i = 0; i < 15; i++)
                data[i] = data2[i];


            b_sum = data[13] ^ data[12];
            b_sum = b_sum + data[11] + data[10] - data[9] - data[8];
            b_sum = b_sum ^ data[7];
            b_sum = b_sum & 0xFF;
            a_sum = data[6] ^ b_sum;
            a_sum = a_sum - data[5] - data[4];
            a_sum = a_sum ^ data[3];
            a_sum = a_sum + data[2] + data[1] - data[0];
            a_sum = a_sum & 0xFF;

            if (a_sum != key[7])
            {
                //if (1)
                //    qDebug() << "checksum mismatch: ";
                return null;
            }

            key_offset = key[1] ^ key[2];
            key_offset = key_offset - key[3] - key[4];
            key_offset = key_offset ^ key[5];
            key_offset = key_offset + key[6] - key[7];
            key_offset = key_offset ^ data[14];
            key_offset = key_offset + 0x26;
            key_offset = key_offset & 0xFF;

            key_index = 4;

            for (x = 12; x >= 0; x--)
            {
                _byte = data[x];

                for (y = 4; y > 1; y--)
                {
                    key_offset--;

                    bkey = KEY_TABLE[key_offset + 0x41];
                    keyr = key[key_index];
                    if (--key_index == 0)
                        key_index = 7;

                    if ((bkey & 3) == 0)
                        _byte = (_byte - bkey) - keyr;
                    else if ((bkey & 3) == 1)
                        _byte = ((_byte + bkey) + keyr);
                    else
                        _byte = ((_byte ^ bkey) ^ keyr);
                }
                ret.Add((byte)_byte);
            }

            return ret;
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
