using MadWizard.WinUSBNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyInterfaceWrap;

namespace GunconUSB
{
    //https://tewarid.github.io/2012/03/23/custom-usb-driver-and-app-using-winusb-and-c.html
    //https://tewarid.github.io/2013/06/19/winusbnet-patch-to-handle-language-id.html
    //https://dzone.com/articles/setting-mouse-cursor-position
    
    //virtual driver for joystick,mouse,keyboard
    //https://tetherscript.com/hid-driver-kit-download/

    public partial class MainForm : Form
    {
        //private readonly GunconReader gconreader = new GunconReader();

        public MainForm()
        {
            InitializeComponent();
            bindEvents();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //GammaManager.SetBrightness(255);
            //Thread.Sleep(20);
            //GammaManager.RestoreBrightness();

            Start();
            
            //this.TopMost = true;
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.WindowState = FormWindowState.Maximized;

            //var calibracao = new UCCallibration { Dock = DockStyle.Fill };
            //this.Controls.Clear();
            //this.Controls.Add(calibracao);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
            unBindEvents();
        }

        private void bindEvents()
        {
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
            GunconReader.ProgressChanged += Gconreader_ProgressChanged;
        }

        private void unBindEvents()
        {
            this.Load -= MainForm_Load;
            this.FormClosing -= MainForm_FormClosing;
            GunconReader.ProgressChanged -= Gconreader_ProgressChanged;
        }



        private void Gconreader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //object userObject = e.UserState;
            //int percentage = e.ProgressPercentage;
            //System.Diagnostics.Debug.WriteLine(userObject);
            //System.Diagnostics.Debug.WriteLine(DateTime.Now);

            if (rbMoveJoy.Checked || rbMoveMouse.Checked)
            {
                Helper.MoveMouse(GunState.PointerX, GunState.PointerY);
                //if (GunState.Trigger)
                //{
                //    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                //    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                //}
            }


            UpdateForm();
        }

        private void Start()
        {
            GunconReader.Start();
        }
        
        private void Stop()
        {
            GunconReader.Stop();
        }

        private void UpdateForm()
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new ThreadStart(UpdateForm), new object[] { });
            //}
            checkBoxRunning.Checked = GunconReader.IsRunning;//isStarted;

            checkBoxA.Checked = GunState.BtnA;
            checkBoxB.Checked = GunState.BtnB;
            checkBoxC.Checked = GunState.BtnC;
            checkBoxTrigger.Checked = GunState.Trigger;
            checkBoxSelect.Checked = GunState.Select;
            checkBoxStart.Checked = GunState.Start;

            numericUpDownPadX.Value = GunState.PadX;
            numericUpDownPadY.Value = GunState.PadY;

            textBoxPointerX.Text = GunState.PointerX.ToString();
            textBoxPointerY.Text = GunState.PointerY.ToString();


            textBoxMinX.Text = GunState.MinX.ToString();
            textBoxMinY.Text = GunState.MinY.ToString();

            textBoxMaxX.Text = GunState.MaxX.ToString();
            textBoxMaxY.Text = GunState.MaxY.ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void btnCalibrate_Click(object sender, EventArgs e)
        {
            //var calib = new UCCallibration { Dock = DockStyle.Fill };
            //this.Controls.Add(calib);
            //return;

            var wasRunning = GunconReader.IsRunning;
            //if (wasRunning)
            //{
            //    GunconReader.Stop();
            //    Thread.Sleep(20);
            //}

            //GunconReader.ProgressChanged -= this.Gconreader_ProgressChanged;

            new CalibrationForm(this.Location) { Owner = this }.Show();

            //GunconReader.ProgressChanged += this.Gconreader_ProgressChanged;

            //if (wasRunning)
            //    GunconReader.Start();
        }

        //static void PrintHex(byte[] data, int length)
        //{
        //    for (int i = 0; i < length; i++)
        //        Console.Write("{0:x2} ", data[i]);
        //    Console.Write(Environment.NewLine);
        //}

        //private void updatemouse()
        //{
        //    if (rbMoveMouse.Checked)
        //    {
        //        var x = GunState.PointerX;
        //        var y = GunState.PointerY;
        //        if (x > 0 && y > 0)
        //        {
        //            Rectangle resolution = Screen.PrimaryScreen.Bounds;
        //            x = Helper.ConvertRange(GunState.MinX, GunState.MaxX, 0, resolution.Width, x);
        //            y = Helper.ConvertRange(GunState.MinY, GunState.MaxY, 0, resolution.Height, y);

        //            //var cursor = new Cursor(Cursor.Current.Handle);
        //            Cursor.Position = new Point(x, y);
        //            //Cursor.Clip = new Rectangle(this.Location, this.Size);
        //        }
        //    }

        //    if (GunState.BtnB)
        //    {
        //        Stop();
        //        //shouldContinue = false;
        //    }
        //}
    }

}
