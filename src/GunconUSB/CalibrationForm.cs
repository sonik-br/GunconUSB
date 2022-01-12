using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace GunconUSB
{
    public partial class CalibrationForm : Form
    {
        private int offSetX, offSetY;
        private Point parentLocation;


        public CalibrationForm(Point parentLocation)
        {
            InitializeComponent();
            this.parentLocation = parentLocation;
            GunconReader.SetOffset(0, 0);
            bindEvents();
        }

        private void bindEvents()
        {
            this.Load += CalibrationForm_Load;
            this.FormClosing += CalibrationForm_FormClosing;
            this.KeyUp += CalibrationForm_KeyUp;
            GunconReader.ProgressChanged += GunconReader_ProgressChanged;
        }

        private void CalibrationForm_Load(object sender, EventArgs e)
        {
            this.Location = parentLocation;

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            //enable mouse movement / calibration mode
            //GunconReader.
            //var inp = new InputSimulator();
        }

        private void CalibrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            unBindEvents();
        }

        private void unBindEvents()
        {
            this.Load -= CalibrationForm_Load;
            this.FormClosing -= CalibrationForm_FormClosing;
            this.KeyUp -= CalibrationForm_KeyUp;
            GunconReader.ProgressChanged -= GunconReader_ProgressChanged;
        }

        private void GunconReader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            //new InputSimulator().Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
            //new InputSimulator().Mouse.

            var x = GunState.PointerX + offSetX;
            var y = GunState.PointerY + offSetY;
            if (x > 0 && y > 0)
            {
                Helper.MoveMouse(x, y);
            }

            if (GunState.Trigger)
            {
                var gunx = GunState.PointerX;
                var gunY = GunState.PointerY;

                var centerX = (GunState.MaxX + GunState.MinX) / 2;// - GunState.MinX; // (GunState.MaxX - GunState.MinX) / 2;
                var centerY = (GunState.MaxY + GunState.MinY) / 2; //(GunState.MaxY - GunState.MinY) / 2;

                offSetX = centerX - gunx;
                offSetY = centerY - gunY;
            }
        }

        private void CalibrationForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                Close();

            if (e.KeyData == Keys.Enter)
            {
                GunconReader.SetOffset((sbyte)offSetX, (sbyte)offSetY);
                Close();
            }
        }
    }
}
