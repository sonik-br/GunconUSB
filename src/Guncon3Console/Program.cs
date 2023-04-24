using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Guncon3Console.TetherScript;

namespace Guncon3Console
{
    /*
    need to create app to map map gun function to keyboard/mouse/joy
    for keyboard, use keycode byte
    BTN_TRIGGER=MOUSE_LEFT
    BTN_A1
    BTN_A2
    BTN_B1=1 //start
    BTN_B2=5 //coint
    BTN_C1=MOUSE_RIGHT
    BTN_C2=MOUSE_MIDDLE

    https://www.flickr.com/photos/playstationblog/1876974892/
    https://upload.wikimedia.org/wikipedia/commons/c/c8/Guncon-3.jpg
    */



    //console sample app from https://stackoverflow.com/questions/474679/capture-console-exit-c-sharp

    class Program
    {
        private static bool debugMode = false;
        private static bool hasError = false;

        // if you want to allow only one instance otherwise remove the next line
        static readonly Mutex mutex = new Mutex(false, "47c2b270-9e34-45e7-82ab-7b29bf54677d");

        static ManualResetEvent run = new ManualResetEvent(true);

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler(CtrlType sig);
        static EventHandler exitHandler;
        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        private static bool ExitHandler(CtrlType sig)
        {
            Console.WriteLine("Shutting down: " + sig.ToString());
            run.Reset();
            Thread.Sleep(2000);
            return false; // If the function handles the control signal, it should return TRUE. If it returns FALSE, the next handler function in the list of handlers for this process is used (from MSDN).
        }

        static void Main(string[] args)
        {
            // if you want to allow only one instance otherwise remove the next 4 lines
            if (!mutex.WaitOne(TimeSpan.FromSeconds(2), false))
                return; // singleton application already started

            var showHelp = args.Contains("help", StringComparer.OrdinalIgnoreCase);
            debugMode = args.Contains("debug", StringComparer.OrdinalIgnoreCase);
            var mode4by3 = args.Contains("43", StringComparer.OrdinalIgnoreCase);
            var showKeys = args.Contains("keys", StringComparer.OrdinalIgnoreCase);

            exitHandler += new EventHandler(ExitHandler);
            SetConsoleCtrlHandler(exitHandler, true);

            try
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.SetBufferSize(Console.BufferWidth, 1024);

                Console.Title = "GUNCON3";

                Console.WriteLine("GUNCON3 V0.01 - BY SONIK");
                Console.WriteLine();


                if (showHelp)
                {
                    Console.WriteLine("Arguments:");
                    Console.WriteLine();
                    Console.WriteLine("43\tEnable 4:3 mode. For use with MAME in 4:3 blackboxed on 16:9 resolution");
                    Console.WriteLine("keys\tShow keycodes for command mapping");
                    Console.WriteLine("debug\tEnable debug information");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                }


                if (showKeys)
                {
                    ShowKeyCodes();
                    Console.WriteLine();
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                }
                

                // start your threads here
                //Thread thread1 = new Thread(new ThreadStart(ThreadFunc1));
                //thread1.Start();

                //Thread thread2 = new Thread(new ThreadStart(ThreadFunc2));
                //thread2.IsBackground = true; // a background thread
                //thread2.Start();


                Console.WriteLine("4:3 inside 16:9 mode enabled");


                Guncon3.Connect();
                AbsMouseFeeder.Connect();
                AbsMouseFeeder.Force4by3 = mode4by3;
                KeyboardFeeder.Connect();
                ReadMappingFile();
                //TetherScriptKeyboardFeeder.Mapping.Add(GunButton.B1, 29 + 1);
                //TetherScriptKeyboardFeeder.Mapping.Add(GunButton.B2, 29 + 5);

                //TetherScriptAbsMouseFeeder.Mapping.Add(GunButton.Trigger, MouseButton.Left);
                //TetherScriptAbsMouseFeeder.Mapping.Add(GunButton.C1, MouseButton.Right);
                //TetherScriptAbsMouseFeeder.Mapping.Add(GunButton.C2, MouseButton.Middle);


                if (File.Exists("calibration.txt"))
                {
                    Console.WriteLine("Using calibration data.");
                    Calibration.Import();
                }
                else
                {
                    Console.WriteLine("No calibration data found. Openning calibration screen...");
                    //while (Console.Read() != (int)ConsoleKey.Enter) { }
                    Calibration.Calibrate();
                    
                    if (Calibration.k_coefs_seted)
                    {
                        Calibration.Export();
                        Console.WriteLine("Calibration data saved.");

                        //Console.WriteLine("Want to save No calibration data? (Y/N)");
                        //var yesno = new string[] { "y", "n" };
                        //var response = Console.ReadLine();
                        //do
                        //{
                        //    if (response.Equals("y", StringComparison.OrdinalIgnoreCase))
                        //    {
                        //        Calibration.Export();
                        //        Console.WriteLine("Calibration data saved.");
                        //        break;
                        //    }
                        //    else if (response.Equals("n", StringComparison.OrdinalIgnoreCase))
                        //    {
                        //        break;
                        //    }
                        //    response = Console.ReadLine();
                        //} while (true);
                    }

                }

                if (!Calibration.k_coefs_seted)
                {
                    Console.WriteLine("Impossible to use without calibration.");
                    hasError = true;
                    return;
                }

                Thread.Sleep(500);

                Console.WriteLine("Ready to use!");

                while (run.WaitOne(0))
                {
                    //var state = Guncon3.Read();
                    Guncon3.Read();

                    AbsMouseFeeder.Feed();
                    KeyboardFeeder.Feed();

                    if (GunState.IsInsideScreen)
                    {
                        //MouseOperations.SetCursorPosition(GunState.ABS_X, GunState.ABS_Y);
                        //vjoyFeeder.Feed(true);
                    }
                    else
                    {
                        //out of screen
                    }
                    /*
                    if (GunState.BTN_TRIGGER != GunState.BTN_TRIGGER_LAST)
                    {
                        if (GunState.BTN_TRIGGER)
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                        else
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
                    }

                    if (GunState.BTN_C1 != GunState.BTN_C1_LAST)
                    {
                        if (GunState.BTN_C1)
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                        else
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);
                    }

                    if (GunState.BTN_C2 != GunState.BTN_C2_LAST)
                    {
                        if (GunState.BTN_C2)
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.MiddleDown);
                        else
                            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.MiddleDown);
                    }
                    */

                    //if (GunState.BTN_TRIGGER)
                    //    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
                    //else if (GunState.BTN_TRIGGER != GunState.BTN_TRIGGER_LAST)
                    //    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);

                    //if (state.C1)
                    //    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightDown);
                    //else
                    //    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.RightUp);

                    //if (state.C2)
                    //    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.MiddleDown);
                    //else
                    //    MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.MiddleUp);

                    //Thread.Sleep(0);
                }

                // do thread syncs here signal them the end so they can clean up or use the manual reset event in them or abort them
                //thread1.Abort();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("fail: ");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("Inner: " + ex.InnerException.Message);
                hasError = true;
            }
            finally
            {
                // do app cleanup here
                Guncon3.Disconnect();
                AbsMouseFeeder.Disconnect();
                KeyboardFeeder.Disconnect();
                Calibration.CloseCalibrationForm();

                if (hasError && debugMode)//dont close console
                {
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                }

                // if you want to allow only one instance otherwise remove the next line
                mutex.ReleaseMutex();

                // remove this after testing
                //Console.Beep(5000, 100);
            }
        }

        private static void ReadMappingFile()
        {
            //todo create default file if not exists
            //if (!File.Exists("mapping.txt"))
            //{

            //}


            if (new FileInfo("mapping.txt").Length > 100000)//prevent if from reading a large file
                throw new Exception("Invalid file size for mapping file");

            var lines = File.ReadAllLines("mapping.txt");
            var typegun = typeof(GunButton);
            var typemouse = typeof(MouseButton);
            byte linecount = 0;
            try
            {
                foreach (var line in lines)
                {
                    linecount++;

                    if (!line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                    {
                        var trimmedLine = line.TrimEnd();
                        var dotIndex = trimmedLine.IndexOf('.');
                        var equalIndex = trimmedLine.IndexOf('=');
                        var mapDevice = trimmedLine.Substring(0, dotIndex);
                        var key = trimmedLine.Substring(dotIndex + 1, equalIndex - dotIndex - 1);
                        var value = trimmedLine.Substring(equalIndex + 1);
                        var gunbtn = (GunButton)Enum.Parse(typegun, value);

                        if (mapDevice == "KEYBOARD")
                            KeyboardFeeder.Mapping.Add(gunbtn, byte.Parse(key));
                        else if (mapDevice == "MOUSE")
                            AbsMouseFeeder.Mapping.Add(gunbtn, (MouseButton)Enum.Parse(typemouse, key));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading mapping file at line {linecount} - {ex.Message}");
            }
        }

        private static void ShowKeyCodes()
        {
            var FKeys = new List<string>();
            FKeys.Add("dummy1");
            FKeys.Add("dummy2");
            FKeys.Add("dummy3");
            FKeys.Add("dummy4");
            FKeys.Add("a");
            FKeys.Add("b");
            FKeys.Add("c");
            FKeys.Add("d");
            FKeys.Add("e");
            FKeys.Add("f");
            FKeys.Add("g");
            FKeys.Add("h");
            FKeys.Add("i");
            FKeys.Add("j");
            FKeys.Add("k");
            FKeys.Add("l");
            FKeys.Add("m");
            FKeys.Add("n");
            FKeys.Add("o");
            FKeys.Add("p");
            FKeys.Add("q");
            FKeys.Add("r");
            FKeys.Add("s");
            FKeys.Add("t");
            FKeys.Add("u");
            FKeys.Add("v");
            FKeys.Add("w");
            FKeys.Add("x");
            FKeys.Add("y");
            FKeys.Add("z");
            FKeys.Add("1");
            FKeys.Add("2");
            FKeys.Add("3");
            FKeys.Add("4");
            FKeys.Add("5");
            FKeys.Add("6");
            FKeys.Add("7");
            FKeys.Add("8");
            FKeys.Add("9");
            FKeys.Add("0");
            FKeys.Add("ENTER");
            FKeys.Add("ESCAPE");
            FKeys.Add("BACKSPACE");
            FKeys.Add("TAB");
            FKeys.Add("SPACEBAR");
            FKeys.Add("-");
            FKeys.Add("=");
            FKeys.Add("[");
            FKeys.Add("]");
            FKeys.Add("\\");
            FKeys.Add("");
            FKeys.Add(";");
            FKeys.Add("dummy5");
            FKeys.Add("`");
            FKeys.Add(",");
            FKeys.Add(".");
            FKeys.Add("/");
            FKeys.Add("CAPSLOCK");
            FKeys.Add("F1");
            FKeys.Add("F2");
            FKeys.Add("F3");
            FKeys.Add("F4");
            FKeys.Add("F5");
            FKeys.Add("F6");
            FKeys.Add("F7");
            FKeys.Add("F8");
            FKeys.Add("F9");
            FKeys.Add("F10");
            FKeys.Add("F11");
            FKeys.Add("F12");
            FKeys.Add("PRINTSCREEN");
            FKeys.Add("SCROLLLOCK");
            FKeys.Add("PAUSE");
            FKeys.Add("INSERT");
            FKeys.Add("HOME");
            FKeys.Add("PAGEUP");
            FKeys.Add("DELETE");
            FKeys.Add("END");
            FKeys.Add("PAGEDOWN");
            FKeys.Add("RIGHTARROW");
            FKeys.Add("LEFTARROW");
            FKeys.Add("DOWNARROW");
            FKeys.Add("UPARROW");
            FKeys.Add("NUMLOCK");
            FKeys.Add("K/");
            FKeys.Add("K*");
            FKeys.Add("K-");
            FKeys.Add("K+");
            FKeys.Add("KENTER");
            FKeys.Add("K1");
            FKeys.Add("K2");
            FKeys.Add("K3");
            FKeys.Add("K4");
            FKeys.Add("K5");
            FKeys.Add("K6");
            FKeys.Add("K7");
            FKeys.Add("K8");
            FKeys.Add("K9");
            FKeys.Add("K0");
            FKeys.Add("K.");
            FKeys.Add("F13");
            FKeys.Add("F14");
            FKeys.Add("F15");
            FKeys.Add("F16");
            FKeys.Add("F17");
            FKeys.Add("F18");
            FKeys.Add("F19");
            FKeys.Add("F20");
            FKeys.Add("F21");
            FKeys.Add("F22");
            FKeys.Add("F23");
            FKeys.Add("F24"); //115

            Console.WriteLine($"KEYCODE\tKEY");
            for (int i = 4; i < FKeys.Count; i++)
                Console.WriteLine($"{i}\t{FKeys[i]}");
        }

    }
}
