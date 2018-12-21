using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Threading.Tasks;


namespace BringAppToForeground
{    
    class Program
    {
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        private const int SW_RESTORE = 9;
        //const string processNameToCheck = "DrawLine2D";

        private static string processNameToCheck;
        private static int intervalBetweenChecksInSecs;

        private static void Main(string[] args)
        {           
            // return if another process of the same name is running
            if (Process.GetProcessesByName(GetAppName()).Length > 1)
            {
                return;
            }


            if (args.Length < 2)
            {
                Console.WriteLine("Wrong usage." + Environment.NewLine
                    + "Usage: " + GetAppName() + " processNameToCheck intervalToCheckInSecs");
            }
            else
            {
                try
                {
                    processNameToCheck = args[0];
                    intervalBetweenChecksInSecs = int.Parse(args[1]);
                    SetAppCheckTimer();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                }
            }

            Console.ReadKey();
        }

        private static void SetAppCheckTimer()
        {
            // Create a timer with a two second interval.
            Timer aTimer = new Timer(intervalBetweenChecksInSecs * 1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void CheckAndBringProcessToFront()
        {
            Process proc = Process.GetProcessesByName(processNameToCheck).FirstOrDefault();

            if (proc != null)
            {
                BringProcessToFront(proc);
            }
            else
            {
                Console.WriteLine("Process {0} not found.", processNameToCheck);
            }
        }

        private static void BringProcessToFront(Process proc)
        {
            IntPtr handle = proc.MainWindowHandle;
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }

            SetForegroundWindow(handle);
        }

        private static string GetAppName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name;
        }


        /* event handlers */

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                CheckAndBringProcessToFront();
                Console.WriteLine("Timed event run successfully.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        /* end of event handlers */
    }
}
