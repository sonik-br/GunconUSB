using MadWizard.WinUSBNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GunconUSB
{
    internal static class GunconReader
    {
        public static bool IsRunning { get; private set; }

        public static event ProgressChangedEventHandler ProgressChanged;


        private const int pid = 362;
        private const int vid = 2970;
        private static USBDevice device = null;
        private static readonly Guid deviceguid = new Guid("{A5DCBF10-6530-11D2-901F-00C04FB951ED}");

        private static BackgroundWorker _workerThread = null;

        private static VJoyFeeder vjoyFeeder = new VJoyFeeder();


        private static sbyte offsetX = 0;
        private static sbyte offsetY = 0;

        private static bool enableBlink = false;
        private static bool needToBlink = false;
        private static int framesOutScreen = 0;

        //private static sbyte _rollingMultiplier = -1;
        //private static sbyte rollingMultiplier
        //{
        //    get { return _rollingMultiplier; }
        //    set
        //    {
        //        _rollingMultiplier = value;
        //        if (_rollingMultiplier < -1)
        //            rollingMultiplier = 4;
        //        if (_rollingMultiplier > 4)
        //            rollingMultiplier = 4;
        //    }
        //}

        //private static byte lastByte3 = 0;


        public static void Start()
        {
            if (_workerThread != null)
                return;

            //vjoyFeeder.initVjoyInterface();

            try
            {
                IsRunning = true;

                var deviceInfo = USBDevice.GetDevices(deviceguid).Where(x => x.PID == pid && x.VID == vid).FirstOrDefault();
                if (deviceInfo == null)
                    throw new Exception("Can't find guncon2 device.");

                device = new USBDevice(deviceInfo);
                
                // x, x, y, y, ?, mode
                byte[] command = new byte[] { 0, 0, 0, 0, 0, 1 };
                //byte[] command = new byte[] { 0x14, 0, 0, 0, 0, 1 };

                device.ControlOut(0x21, 0x09, 0x200, 0, command);

                //if (initVjoyInterface())//try to initialize vjoy
                //    joystick.ResetVJD(id);// Reset this device to default values
                //else
                //    joystick = null;
            }
            catch (Exception ex)
            {
                if (device != null)
                {
                    device.Dispose();
                    device = null;
                }

                IsRunning = false;
                ProgressChanged?.Invoke(null, null);
                return;
            }


            _workerThread = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _workerThread.DoWork += BackgroundWorkerOnDoWork;
            _workerThread.ProgressChanged += BackgroundWorkerOnProgressChanged;
            _workerThread.RunWorkerCompleted += _workerThread_RunWorkerCompleted;
            _workerThread.RunWorkerAsync();

        }

        public static void Stop()
        {
            if (_workerThread == null)
                return;

            _workerThread.CancelAsync();


            //if (joystick != null)
            //{
            //    joystick.ResetVJD(id);
            //    if (joystick.GetVJDStatus(id) == VjdStat.VJD_STAT_OWN)
            //        joystick.RelinquishVJD(id);
            //    joystick = null;
            //}

        }

        private static void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            while (!worker.CancellationPending)
            {
                read();

                //vjoyFeeder.Feed(true);

                //if (rbMoveMouse.Checked)
                //{
                //    var x = GunState.PointerX;
                //    var y = GunState.PointerY;
                //    if (x > 0 && y > 0)
                //    {
                //        Rectangle resolution = Screen.PrimaryScreen.Bounds;
                //        x = Helper.ConvertRange(GunState.MinX, GunState.MaxX, 0, resolution.Width, x);
                //        y = Helper.ConvertRange(GunState.MinY, GunState.MaxY, 0, resolution.Height, y);

                //        //var cursor = new Cursor(Cursor.Current.Handle);
                //        Cursor.Position = new Point(x, y);
                //        //Cursor.Clip = new Rectangle(this.Location, this.Size);
                //    }
                //}

                //worker.ReportProgress(0, "AN OBJECT TO PASS TO THE UI-THREAD");
                worker.ReportProgress(0);
                Thread.Sleep(1);
            }
        }

        private static void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(sender, e);
            //object userObject = e.UserState;
            //int percentage = e.ProgressPercentage;
            //System.Diagnostics.Debug.WriteLine(userObject);
        }

        private static void _workerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _workerThread.DoWork -= BackgroundWorkerOnDoWork;
            _workerThread.ProgressChanged -= BackgroundWorkerOnProgressChanged;
            _workerThread.RunWorkerCompleted -= _workerThread_RunWorkerCompleted;

            if (device != null)
            {
                device.Dispose();
                device = null;
            }

            IsRunning = false;
            _workerThread = null;
            ProgressChanged?.Invoke(null, null);
        }


        public static void SetOffset(sbyte x, sbyte y)
        {
            offsetX = x;
            offsetY = y;
        }


        private static void read()
        {
            //if (!timer.Enabled)
            //    return;

            USBInterface iface = device.Interfaces[0];

            byte[] data = new byte[6];

            //if (!timer.Enabled)
            //    return;

            int len = iface.InPipe.Read(data);

            //Console.WriteLine(string.Join(", ", data));

            //PrintHex(data, len);

            GunState.BtnA = (data[0] & 0x08) == 0;
            GunState.BtnB = (data[0] & 0x04) == 0;
            GunState.BtnC = (data[0] & 0x02) == 0;

            GunState.Trigger = (data[1] & 0x20) == 0;
            GunState.Start = (data[1] & 0x80) == 0;
            GunState.Select = (data[1] & 0x40) == 0;

            //HAT
            if ((data[0] & 0x10) == 0)
                GunState.PadX = -1;
            else if ((data[0] & 0x40) == 0)
                GunState.PadX = 1;
            else
                GunState.PadX = 0;

            if ((data[0] & 0x80) == 0)
                GunState.PadY = -1;
            else if ((data[0] & 0x20) == 0)
                GunState.PadY = 1;
            else
                GunState.PadY = 0;

            if (needToBlink)
            {
                GammaManager.SetBrightness(255);
                Thread.Sleep(40);
                len = iface.InPipe.Read(data);
                GammaManager.RestoreBrightness();
                Thread.Sleep(2);
                needToBlink = false;
            }
            else
            {
                if (GunState.Trigger && enableBlink)
                    needToBlink = true;
            }


            //System.Diagnostics.Debug.WriteLine(data[0] & 0x10);

            uint gunX;
            gunX = data[3];
            gunX <<= 8;
            gunX |= data[2];


            uint gunY;
            gunY = data[5];
            gunY <<= 8;
            gunY |= data[4];

            var lastPointerX = GunState.PointerX;
            
            GunState.PointerX = (int)gunX;
            GunState.PointerY = (int)gunY;

            System.Diagnostics.Debug.WriteLine($"{data[4]}\t{data[5]}");
            //System.Diagnostics.Debug.WriteLine($"{data[2]} {data[3]}");
            //System.Diagnostics.Debug.WriteLine("--------------------");

            //if (true)//tentative to fix rolling x problem
            //{
            //    if (data[3] != lastByte3)
            //    {
            //        if (data[3] > lastByte3)
            //        {
            //            if (data[3] == 2 && lastByte3 == 0)
            //                rollingMultiplier--;
            //            else
            //                rollingMultiplier++;
            //        }
            //        else
            //        {
            //            if (data[3] == 0 && lastByte3 == 2)
            //                rollingMultiplier++;
            //            else
            //                rollingMultiplier--;
            //        }
            //        GunState.PointerX = data[2] + (255 * rollingMultiplier);
            //    }
            //    else
            //    {
            //        if (data[2] == 1 && data[3] == 0)
            //            GunState.PointerX = 0;
            //        else if (rollingMultiplier != -1)
            //            GunState.PointerX = data[2] + (255 * rollingMultiplier);
            //    }
            //    lastByte3 = data[3];
            //}


            if (GunState.PointerX > 10)
                GunState.MinX = Math.Min(GunState.MinX, GunState.PointerX);
            if (GunState.PointerY > 5)
                GunState.MinY = Math.Min(GunState.MinY, GunState.PointerY);

            if (GunState.PointerX == 0 && GunState.PointerY > 0)
            {
                framesOutScreen++;
                if (framesOutScreen > -1)
                {
                    if (framesOutScreen < 10)
                    {
                        GunState.PointerX = lastPointerX;
                    }
                    else//out of screen
                    {
                        //lastByte3 = 0;
                        //rollingMultiplier = -1;
                    }
                }
            }
            else
            {
                framesOutScreen = -1;
            }

            GunState.MaxX = Math.Max(GunState.MaxX, GunState.PointerX);
            GunState.MaxY = Math.Max(GunState.MaxY, GunState.PointerY);

            GunState.PointerX += offsetX;
            GunState.PointerY += offsetY;
        }
    }
}
