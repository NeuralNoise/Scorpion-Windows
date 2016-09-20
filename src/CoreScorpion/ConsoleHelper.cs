using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    static class ConsoleHelper
    {
        public static int EXIT_SUCCESS = 0;
        public static int EXIT_FAILURE = 1;

        public static void Error(string format, params object[] args)
        {
            string message = string.Format(format, args);
            Console.Out.Write(string.Format(
                "{0}: {1}\n", Application.Constants.Name, message));
            Exit(EXIT_FAILURE);
        }

        public static void Exit(int val)
        {
            if(Application.Constants.Wait)
            {
                Console.Out.Write("press any key to continue:");
                Console.Read();
            }

            System.Environment.Exit(val);
        }

        public static string[] ProcessArgs(string[] args)
        {
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].ElementAt(0) != '-')
                    {
                        int argc = args.Length - i, iter = 0;
                        string[] lst = new string[argc];

                        do
                        {
                            lst[iter++] = args[i++];
                        } while (i < args.Length);

                        return lst;
                    }
                    else if (args[i] == "-version" ||
                        args[i] == "-showversion")
                    {
                        Console.Out.WriteLine(Application.Constants.Name + " version \"" +
                            Application.Constants.Version + "\"");
                        Console.Out.WriteLine(Application.Constants.ProductNameShort +
                            " (build SCORPION_VERSION_" + Application.Constants.Version +
                            " -b " + Application.Constants.BuildVersion + ")");
                        Console.Out.WriteLine(Application.Constants.ProductNameShort + " " +
                            Application.Constants.TargetOs + " " + Application.Constants.TargetMachine +
                            "Server VM (v" + Application.Constants.BuildVersion + ")");

                        if (args[i] == "-version")
                        {
                            Exit(EXIT_SUCCESS);
                        }
                    }
                    else if (args[i] == "-help" || args[i] == "-?")
                    {
                        Usage();
                    }
                    else if (args[i] == "-da")
                    {
                        Globals.AssertEnabled = false;
                    }
                    else if (args[i] == "-l")
                    {
                        Globals.ShowExe = true;
                    }
                    else if (args[i] == "-dbg")
                    {
                        Globals.DebugEnabled = true;
                    }
                    else if (args[i] == "-log=off")
                    {
                        Globals.Logger.level = Log.OFF;
                    }
                    else if (args[i] == "-log=verbose")
                    {
                        Globals.Logger.level = Log.VERBOSE;
                    }
                    else if (args[i] == "-log=debug")
                    {
                        Globals.Logger.level = Log.DEBUG;
                    }
                    else if (args[i] == "-log=info")
                    {
                        Globals.Logger.level = Log.INFO;
                    }
                    else if (args[i] == "-log=warn")
                    {
                        Globals.Logger.level = Log.WARN;
                    }
                    else if (args[i] == "-log=error")
                    {
                        Globals.Logger.level = Log.ERROR;
                    }
                    else if (args[i] == "-log=assert")
                    {
                        Globals.Logger.level = Log.ASSERT;
                    }
                    else
                    {
                        string error = "Unrecognized command line option: " + args[i];
                        error += "\nTry '" + Application.Constants.Name + " -help' for more information.";

                        Error(error);
                    }
                }

                return null;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private static void Usage()
        {
            // TODO: Print usage
            Exit(EXIT_SUCCESS);
        }
    }
}
