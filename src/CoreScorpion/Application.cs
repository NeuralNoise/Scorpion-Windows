using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    static class Application
    {
        public static class Constants
        {
            public static int MachineVersion = 1;
            public static int BuildVersion = 0100;
            public static string Version = "0.1.0.0";
            public static string Name = "scorpion";
            public static string ProductName = "Scorpio(TM) Runtime Environment (SE)";
            public static string ProductNameShort = "Scorpio(TM) (SE)";
            public static string TargetOs = "windows";
            public static string TargetMachine = "64Bit";
            public static string Environment = "std";

            public static bool Wait = true;
        }

        public static bool Ready()
        {
            return true;
        }

        public static void Error(string message)
        {
            Console.Out.WriteLine(Constants.ProductName + " failed to start.");
            Console.Out.WriteLine("error: " + message);

            ConsoleHelper.Exit(ConsoleHelper.EXIT_FAILURE);
        }
    }
}
