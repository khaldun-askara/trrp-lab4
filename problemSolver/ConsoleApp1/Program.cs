using System;
using System.IO;
using System.Timers;
using System.Configuration;
using System.Collections.Specialized;
using ConsoleApp1;

namespace ConsoleApplication2
{
    class Program
    {
        static string filepath = @".\lastRunTime.txt";
        static void Main(string[] args)
        {
            Timer t1 = new Timer();
            t1.Interval = (1000 * 60 * 20);
            t1.Elapsed += new ElapsedEventHandler(t1_Elapsed);
            t1.AutoReset = true;
            t1.Start();

            Console.ReadLine();
        }

        static void t1_Elapsed(object sender, ElapsedEventArgs e)
        {
            int hours = Settings1.Default.backuptime;

            DateTime scheduledRun = DateTime.Today.AddHours(hours);  // runs today at 3am
            System.IO.FileInfo lastTime = new System.IO.FileInfo(filepath);
            DateTime lastRan = lastTime.LastWriteTime;
            if (DateTime.Now > scheduledRun)
            {
                TimeSpan sinceLastRun = DateTime.Now - lastRan;
                if (sinceLastRun.Hours > 23)
                {
                    doStuff();
                    using StreamWriter file = new StreamWriter(filepath);
                    file.WriteLine(DateTime.Now);

                }
            }
        }

        static void doStuff()
        {
            Console.WriteLine("Running the method!");
        }
    }
}