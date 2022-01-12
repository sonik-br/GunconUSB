using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Guncon3Calibration
{
    public partial class Form1 : Form
    {
        private readonly string pipeHandle;
        private sbyte currentPass = -1;

        private Bitmap image_diag = null;
        private Bitmap image_ortho = null;
        private Bitmap image_middle = null;

        private static BackgroundWorker _workerThread = null;

        public Form1(string[] args)
        {
            InitializeComponent();

            if (args.Length != 1)
                Close();

            pipeHandle = args[0];

            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_workerThread != null)
            {
                _workerThread.CancelAsync();
                _workerThread.DoWork -= BackgroundWorkerOnDoWork;
                Thread.Sleep(50);
            }
            pictureBox1.Image = null;
            image_diag?.Dispose();
            image_ortho?.Dispose();
            image_middle?.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            var rm = Properties.Resources.ResourceManager;
            image_diag = (Bitmap)rm.GetObject("diag");
            image_ortho = (Bitmap)rm.GetObject("ortho");
            image_middle = (Bitmap)rm.GetObject("middle");

            _workerThread = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };

            TargetNext();

            //while (currentPass < 8)
            //{
            //    await Task.Delay(500);
            //    TargetNext();
            //}

            _workerThread.DoWork += BackgroundWorkerOnDoWork;
            _workerThread.RunWorkerAsync();
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (PipeStream pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeHandle))
                {
                    using (StreamReader sr = new StreamReader(pipeClient, Encoding.ASCII))
                    {
                        while (!_workerThread.CancellationPending)
                        {
                            if (sr.Read() == -1)
                            {
                                Close();
                                break;
                            }
                            else
                            {
                                TargetNext();
                            }
                            Thread.Sleep(5);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Invoke((Action)(() => { label1.Text = ex.Message; pictureBox1.Visible = false; }));
            }
        }

        private void TargetNext()
        {
            currentPass++;

            byte line;
            byte column;
            AnchorStyles anchor;

            var i = currentPass;

            column = (byte)(i % 3);

            if (i < 3)
                line = 1;
            else if (i < 6)
                line = 2;
            else
                line = 3;

            if (column == 0)
                anchor = AnchorStyles.Left;
            else if (column == 1)
                anchor = AnchorStyles.None;
            else
                anchor = AnchorStyles.Right;

            if (line == 1)
                anchor |= AnchorStyles.Top;
            else if (line == 2)
                anchor |= AnchorStyles.None;
            else
                anchor |= AnchorStyles.Bottom;

            //c.PointValue = (i + 1).ToString();

            //tableLayoutPanel1.SetRow(calibrationPoint1, line - 1);
            //tableLayoutPanel1.SetColumn(calibrationPoint1, column);

            //calibrationPoint1.SetPoint(i + 1, line - 1, column);
            //calibrationPoint1.Anchor = anchor;

            tableLayoutPanel1.SetRow(pictureBox1, line - 1);
            tableLayoutPanel1.SetColumn(pictureBox1, column);

            //calibrationPoint1.SetPoint(i + 1, line - 1, column);
            pictureBox1.Anchor = anchor;

            if (anchor == (AnchorStyles.Top | AnchorStyles.Left))
            {
                pictureBox1.Image = image_diag;
            }
            else if (anchor == (AnchorStyles.Top | AnchorStyles.Right))
            {
                image_diag.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pictureBox1.Image = image_diag;
            }
            else if (anchor == (AnchorStyles.Bottom | AnchorStyles.Left))
            {
                image_diag.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                pictureBox1.Image = image_diag;
            }
            else if (anchor == (AnchorStyles.Bottom | AnchorStyles.Right))
            {
                image_diag.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pictureBox1.Image = image_diag;
            }

            else if (anchor == AnchorStyles.Top)
            {
                pictureBox1.Image = image_ortho;
            }
            else if (anchor == AnchorStyles.Left)
            {
                image_ortho.RotateFlip(RotateFlipType.Rotate270FlipNone);
                pictureBox1.Image = image_ortho;
            }
            else if (anchor == AnchorStyles.Right)
            {
                image_ortho.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pictureBox1.Image = image_ortho;
            }
            else if (anchor == AnchorStyles.Bottom)
            {
                image_ortho.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox1.Image = image_ortho;
            }

            else
            {
                pictureBox1.Image = image_middle;
            }
        }
    }
}
