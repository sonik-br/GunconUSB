using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Guncon3Console
{
    internal static class Calibration
    {
        static Process pipeClient = null;

        const int N = 9;

        static bool trigger_act;
        static int num_samples;
        static sbyte num_sample_points;
        static sbyte num_reference_points;
        static short[,] ReferencePoint = new short[N, 2]; // ideal position of reference points
        static short[,] SamplePoint = new short[N, 2]; // sampling position of reference points
        static double KX1, KX2, KX3, KY1, KY2, KY3; // coefficients for calibration algorithm
        internal static bool k_coefs_seted;
        internal static bool calibracion;

        //static int h_screen, w_screen;

        //static int[,] pixmap_points = new int[N, 2];
        static int[,] ref_points = new int[N, 2];

        //internal static void OpenCalibrationForm()
        //{
        //    ProcessStartInfo startinfo = new ProcessStartInfo("Guncon3Calibration.exe");
        //    startinfo.CreateNoWindow = false;
        //    startinfo.UseShellExecute = false;
        //    pipeClient = Process.Start(startinfo);
        //}

        internal static void CloseCalibrationForm()
        {
            try
            {
                calibracion = false;
                if (pipeClient != null)
                {
                    pipeClient.Close();
                    Thread.Sleep(100);
                    pipeClient.Dispose();
                    pipeClient = null;
                }
            }
            catch (Exception)
            {

            }
        }

        internal static void Calibrate()
        {
            try
            {
                Console.WriteLine("Calibration mode");
                createRefPoints();
                calibracion = true;
                using (AnonymousPipeServerStream pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable))
                {
                    var startinfo = new ProcessStartInfo("Guncon3Calibration.exe");
                    startinfo.CreateNoWindow = false;
                    startinfo.UseShellExecute = false;
                    startinfo.Arguments = pipeServer.GetClientHandleAsString();
                    
                    pipeClient = new Process();
                    pipeClient.StartInfo = startinfo;
                    pipeClient.Start();

                    pipeServer.DisposeLocalCopyOfClientHandle();
                    Thread.Sleep(500);

                    while (calibracion)
                    {
                        if (pipeClient.HasExited)
                            break;

                        //var state = Guncon3.Read();
                        Guncon3.Read();

                        var triggerState = GunState.BtnState[GunButton.Trigger];
                        if (triggerState != trigger_act)
                        {
                            if (!GunState.INDICATOR2 && triggerState)
                            {
                                append_sample_point(GunState.ABS_X, GunState.ABS_Y);
                                num_samples++;

                                if (num_samples > 8)
                                {
                                    Console.WriteLine("Calibration finished");
                                    calibracion = false;
                                }
                                else
                                {
                                    //move calibration target
                                    Console.WriteLine($"Calibration point {num_samples}");
                                    sendPipe(pipeServer, num_samples);
                                }
                            }
                        }
                        trigger_act = triggerState;
                        Thread.Sleep(5);
                    }
                }
            }
            catch (Exception ex)
            {


            }
            finally
            {
                CloseCalibrationForm();
            }
        }

        //private static void createRefPoints()
        //{
        //    //var resolucion = new Point(1920, 1080);
        //    //var resolution = Screen.PrimaryScreen.Bounds;
        //    var resolution = Helper.GetScreenResolution();

        //    int width_screen = resolution.Item1;
        //    int height_screen = resolution.Item2;

        //    int width_pix, height_pix;
        //    w_screen = width_screen;
        //    h_screen = height_screen;

        //    height_pix = 50;//height_screen / 5;
        //    width_pix = height_pix;
        //    //el tamaño de la imagen de la mirilla será un quinto de la resolucion vertical
        //    //ui->pixmap_mirilla->resize(width_pix, height_pix);

        //    //calcular los puntos
        //    ref_points[0, 0] = height_pix / 2; ref_points[0, 1] = height_pix / 2;
        //    ref_points[1, 0] = width_screen / 2; ref_points[1, 1] = height_pix / 2;
        //    ref_points[2, 0] = width_screen - height_pix / 2; ref_points[2, 1] = height_pix / 2;

        //    ref_points[3, 0] = height_pix / 2; ref_points[3, 1] = height_screen / 2;
        //    ref_points[4, 0] = width_screen / 2; ref_points[4, 1] = height_screen / 2;
        //    ref_points[5, 0] = width_screen - height_pix / 2; ref_points[5, 1] = height_screen / 2;

        //    ref_points[6, 0] = height_pix / 2; ref_points[6, 1] = height_screen - height_pix / 2;
        //    ref_points[7, 0] = width_screen / 2; ref_points[7, 1] = height_screen - height_pix / 2;
        //    ref_points[8, 0] = width_screen - height_pix / 2; ref_points[8, 1] = height_screen - height_pix / 2;

        //    pixmap_points[0, 0] = 0; pixmap_points[0, 1] = 0;
        //    pixmap_points[1, 0] = width_screen / 2 - height_pix / 2; pixmap_points[1, 1] = 0;
        //    pixmap_points[2, 0] = width_screen - height_pix; pixmap_points[2, 1] = 0;

        //    pixmap_points[3, 0] = 0; pixmap_points[3, 1] = height_screen / 2 - height_pix / 2;
        //    pixmap_points[4, 0] = width_screen / 2 - height_pix / 2; pixmap_points[4, 1] = height_screen / 2 - height_pix / 2;
        //    pixmap_points[5, 0] = width_screen - height_pix; pixmap_points[5, 1] = height_screen / 2 - height_pix / 2;

        //    pixmap_points[6, 0] = 0; pixmap_points[6, 1] = height_screen - height_pix;
        //    pixmap_points[7, 0] = width_screen / 2 - height_pix / 2; pixmap_points[7, 1] = height_screen - height_pix;
        //    pixmap_points[8, 0] = width_screen - height_pix; pixmap_points[8, 1] = height_screen - height_pix;

        //    //almacenar posiciones
        //    for (int i = 0; i < 9; i++)
        //        append_reference_point(ref_points[i, 0], ref_points[i, 1]);//primer punto, arriba izq pixmap en 0,0
        //}

        private static void createRefPoints()
        {
            //possible mouse abs values
            var min = 0;
            var mid = 16384;
            var max = 32767;

            //calcular los puntos
            ref_points[0, 0] = min; ref_points[0, 1] = min;
            ref_points[1, 0] = mid; ref_points[1, 1] = min;
            ref_points[2, 0] = max; ref_points[2, 1] = min;

            ref_points[3, 0] = min; ref_points[3, 1] = mid;
            ref_points[4, 0] = mid; ref_points[4, 1] = mid;
            ref_points[5, 0] = max; ref_points[5, 1] = mid;

            ref_points[6, 0] = min; ref_points[6, 1] = max;
            ref_points[7, 0] = mid; ref_points[7, 1] = max;
            ref_points[8, 0] = max; ref_points[8, 1] = max;

            //almacenar posiciones
            for (int i = 0; i < 9; i++)
                append_reference_point(ref_points[i, 0], ref_points[i, 1]);//primer punto, arriba izq pixmap en 0,0
        }



        private static void sendPipe(AnonymousPipeServerStream pipeServer, int command)
        {
            using (StreamWriter sw = new StreamWriter(pipeServer, Encoding.ASCII, 1, true))
            {
                sw.AutoFlush = true;
                sw.Write(command);
                pipeServer.WaitForPipeDrain();
            }
        }


        private static sbyte append_reference_point(long x, long y)
        {
            if (num_reference_points >= N)
                return -1; //demasiados puntos de referencia

            ReferencePoint[num_reference_points, 0] = (short)x;
            ReferencePoint[num_reference_points, 1] = (short)y;
            num_reference_points++;
            return 0;
        }

        internal static void Export()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < SamplePoint.Length / 2; i++)
                sb.AppendLine($"{SamplePoint[i, 0]};{SamplePoint[i, 1]}");
            File.WriteAllText("calibration.txt", sb.ToString());
        }

        internal static void Import()
        {
            var contents = File.ReadAllLines("calibration.txt");
            createRefPoints();
            for (int i = 0; i < contents.Length; i++)
            {
                var splitContent = contents[i].Split(';');

                append_sample_point(short.Parse(splitContent[0]), short.Parse(splitContent[1]));
            }
        }

        private static sbyte append_sample_point(long x, long y)
        {
            if (num_sample_points >= N)
                return -1; //demasiados puntos de referencia

            SamplePoint[num_sample_points, 0] = (short)x;
            SamplePoint[num_sample_points, 1] = (short)y;
            num_sample_points++;
            if ((num_sample_points == N) && (num_reference_points == N))
            {
                Get_Calibration_Coefficient();
                k_coefs_seted = true;
            }
            return 0;
        }

        private static int Get_Calibration_Coefficient()
        {
            int i;
            int Points = N;
            var a = new double[3];
            var b = new double[3];
            var c = new double[3];
            var d = new double[3];
            double k;

            if (Points < 3)
            {
                return 0;
            }
            else
            {
                if (Points == 3)
                {
                    for (i = 0; i < Points; i++)
                    {
                        a[i] = (double)(SamplePoint[i, 0]);
                        b[i] = (double)(SamplePoint[i, 1]);
                        c[i] = (double)(ReferencePoint[i, 0]);
                        d[i] = (double)(ReferencePoint[i, 1]);
                    }
                }
                else if (Points > 3)
                {
                    for (i = 0; i < 3; i++)
                    {
                        a[i] = 0;
                        b[i] = 0;
                        c[i] = 0;
                        d[i] = 0;
                    }
                    for (i = 0; i < Points; i++)

                    {
                        a[2] = a[2] + (double)(SamplePoint[i, 0]);
                        b[2] = b[2] + (double)(SamplePoint[i, 1]);
                        c[2] = c[2] + (double)(ReferencePoint[i, 0]);
                        d[2] = d[2] + (double)(ReferencePoint[i, 1]);
                        a[0] = a[0] + (double)(SamplePoint[i, 0]) * (double)(SamplePoint[i, 0]);
                        a[1] = a[1] + (double)(SamplePoint[i, 0]) * (double)(SamplePoint[i, 1]);
                        b[0] = a[1];
                        b[1] = b[1] + (double)(SamplePoint[i, 1]) * (double)(SamplePoint[i, 1]);
                        c[0] = c[0] + (double)(SamplePoint[i, 0]) * (double)(ReferencePoint[i, 0]);
                        c[1] = c[1] + (double)(SamplePoint[i, 1]) * (double)(ReferencePoint[i, 0]);
                        d[0] = d[0] + (double)(SamplePoint[i, 0]) * (double)(ReferencePoint[i, 1]);
                        d[1] = d[1] + (double)(SamplePoint[i, 1]) * (double)(ReferencePoint[i, 1]);
                    }
                    a[0] = a[0] / a[2];
                    a[1] = a[1] / b[2];
                    b[0] = b[0] / a[2];
                    b[1] = b[1] / b[2];
                    c[0] = c[0] / a[2];
                    c[1] = c[1] / b[2];
                    d[0] = d[0] / a[2];
                    d[1] = d[1] / b[2];
                    a[2] = a[2] / Points;
                    b[2] = b[2] / Points;
                    c[2] = c[2] / Points;
                    d[2] = d[2] / Points;
                }
                k = (a[0] - a[2]) * (b[1] - b[2]) - (a[1] - a[2]) * (b[0] - b[2]);
                KX1 = ((c[0] - c[2]) * (b[1] - b[2]) - (c[1] - c[2]) * (b[0] - b[2])) / k;
                KX2 = ((c[1] - c[2]) * (a[0] - a[2]) - (c[0] - c[2]) * (a[1] - a[2])) / k;
                KX3 = (b[0] * (a[2] * c[1] - a[1] * c[2]) + b[1] * (a[0] * c[2] - a[2] * c[0]) + b[2] * (a[1] * c[0] -
                        a[0] * c[1])) / k;
                KY1 = ((d[0] - d[2]) * (b[1] - b[2]) - (d[1] - d[2]) * (b[0] - b[2])) / k;
                KY2 = ((d[1] - d[2]) * (a[0] - a[2]) - (d[0] - d[2]) * (a[1] - a[2])) / k;
                KY3 = (b[0] * (a[2] * d[1] - a[1] * d[2]) + b[1] * (a[0] * d[2] - a[2] * d[0]) + b[2] * (a[1] * d[0] -
                        a[0] * d[1])) / k;
                return Points;
            }
        }

        internal static sbyte Do_Calibration(ref short Px, ref short Py)
        {
            if (k_coefs_seted)
            {
                Px = (short)(KX1 * (Px) + KX2 * (Py) + KX3 + 0.5);
                Py = (short)(KY1 * (Px) + KY2 * (Py) + KY3 + 0.5);
                return 0;
            }
            else
            { return -1; }
        }

    }
}
