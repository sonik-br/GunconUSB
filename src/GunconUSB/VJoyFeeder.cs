using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace GunconUSB
{
    internal class tempText
    {
        public string Text { get; set; }
    }


    class VJoyFeeder
    {
        // Declaring one joystick (Device id 1) and a position structure. 
        private vJoy joystick;
        private vJoy.JoystickState iReport;
        private uint id = 1;
        private int joyAxisMin;
        private int joyAxisMax;

        tempText textBoxVjoyInfo = new tempText();

        public bool initVjoyInterface()
        {

            // Create one joystick object and a position structure.
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();


            // Device ID can only be in the range 1-16
            //if (args.Length > 0 && !String.IsNullOrEmpty(args[0]))
            //    id = Convert.ToUInt32(args[0]);
            if (id <= 0 || id > 16)
            {
                Console.WriteLine("Illegal device ID {0}\nExit!", id);
                return false;
            }

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!joystick.vJoyEnabled())
            {
                textBoxVjoyInfo.Text = "vJoy driver not enabled: Failed Getting vJoy attributes.";
                Console.WriteLine(textBoxVjoyInfo.Text);
                return false;
            }
            else
            {
                textBoxVjoyInfo.Text = $"vJoy driver detected: Vendor: {joystick.GetvJoyManufacturerString()}; Product :{joystick.GetvJoyProductString()}; Version Number:{joystick.GetvJoySerialNumberString()}";
                Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", joystick.GetvJoyManufacturerString(), joystick.GetvJoyProductString(), joystick.GetvJoySerialNumberString());
            }




            // Get the state of the requested device
            VjdStat status = joystick.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", id);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free\n", id);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    textBoxVjoyInfo.Text = $"vJoy Device {id} is already owned by another feeder. Cannot continue";
                    Console.WriteLine(textBoxVjoyInfo.Text);
                    return false;
                case VjdStat.VJD_STAT_MISS:
                    textBoxVjoyInfo.Text = $"vJoy Device {0} is not installed or disabled. Cannot continue";
                    Console.WriteLine(textBoxVjoyInfo.Text);
                    return false;
                default:
                    textBoxVjoyInfo.Text = $"vJoy Device {0} general error. Cannot continue";
                    Console.WriteLine(textBoxVjoyInfo.Text);
                    return false;
            };

            // Check which axes are supported
            bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
            bool AxisZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
            bool AxisRX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
            bool AxisRZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            int nButtons = joystick.GetVJDButtonNumber(id);
            int ContPovNumber = joystick.GetVJDContPovNumber(id);
            int DiscPovNumber = joystick.GetVJDDiscPovNumber(id);

            long jmin = 0, jmax = 0;
            joystick.GetVJDAxisMin(id, HID_USAGES.HID_USAGE_X, ref jmin);
            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref jmax);
            joyAxisMin = (int)jmin;
            joyAxisMax = (int)jmax;

            // Print results
            Console.WriteLine("\nvJoy Device {0} capabilities:\n", id);
            Console.WriteLine("Numner of buttons\t\t{0}\n", nButtons);
            Console.WriteLine("Numner of Continuous POVs\t{0}\n", ContPovNumber);
            Console.WriteLine("Numner of Descrete POVs\t\t{0}\n", DiscPovNumber);
            Console.WriteLine("Axis X\t\t{0}\n", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Y\t\t{0}\n", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Z\t\t{0}\n", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Rx\t\t{0}\n", AxisRX ? "Yes" : "No");
            Console.WriteLine("Axis Rz\t\t{0}\n", AxisRZ ? "Yes" : "No");

            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
            {
                Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", DllVer);
                textBoxVjoyInfo.Text += $"\r\nVersion of Driver Matches DLL Version ({DllVer:X})";
            }
            else
            {
                Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer);
                textBoxVjoyInfo.Text += $".\r\nVersion of Driver ({DrvVer:X}) does NOT match DLL Version ({DllVer:X})";
            }


            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
            {
                Console.WriteLine("Failed to acquire vJoy device number {0}.\n", id);
                textBoxVjoyInfo.Text += $".\r\nFailed to acquire vJoy device number {id}.";
                return false;
            }
            else
                Console.WriteLine("Acquired: vJoy device number {0}.\n", id);

            return true;
        }

        public void Feed(bool rbMoveJoyChecked)
        {
            if (joystick != null)
            {
                bool res;
                //if (GunState.PadX == 0 && GunState.PadY == 0)
                if (GunState.PadX == -1)
                    joystick.SetDiscPov(3, id, 1);
                else if (GunState.PadX == 1)
                    joystick.SetDiscPov(1, id, 1);
                else if (GunState.PadY == -1)
                    joystick.SetDiscPov(2, id, 1);
                else if (GunState.PadY == 1)
                    joystick.SetDiscPov(0, id, 1);
                else
                    joystick.SetDiscPov(-1, id, 1);

                res = joystick.SetBtn(GunState.Trigger, id, 1);
                res = joystick.SetBtn(GunState.BtnA, id, 2);
                res = joystick.SetBtn(GunState.BtnB, id, 2);
                res = joystick.SetBtn(GunState.BtnC, id, 3);

                if (rbMoveJoyChecked)
                {
                    var x = GunState.PointerX;
                    var y = GunState.PointerY;
                    x = Helper.ConvertRange(GunState.MinX, GunState.MaxX, joyAxisMin, joyAxisMax, x);
                    y = Helper.ConvertRange(GunState.MinY, GunState.MaxY, joyAxisMin, joyAxisMax, y);

                    //0 to 32767
                    res = joystick.SetAxis(x, id, HID_USAGES.HID_USAGE_X);
                    res = joystick.SetAxis(y, id, HID_USAGES.HID_USAGE_Y);
                }
            }
        }
    }
}
