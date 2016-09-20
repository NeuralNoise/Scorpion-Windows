using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    class Log
    {
        public static int OFF = 0;
        public static int VERBOSE = 2;
        public static int DEBUG = 3;
        public static int INFO = 4;
        public static int WARN = 5;
        public static int ERROR = 6;
        public static int ASSERT = 7;

        public int level;
        private File file;
        private string stack;

        public Log(string file)
        {
            this.file = new File(file);
            this.stack = "";

            if(level != OFF)
            {
                if(!this.file.Write(""))
                {
                    Console.Out.Write("warning: io error, log file cold not be written to.");
                    this.level = OFF;
                    return;
                }
            }

            stack += "############### System Log ###############\n";
        }
        
        private char levelToChar(int l)
        {
            switch (l)
            {
                case 2:
                    return 'V';
                case 3:
                    return 'D';
                case 4:
                    return 'I';
                case 5:
                    return 'W';
                case 6:
                    return 'E';
                case 7:
                    return 'A';
                default:
                    return '?';
            }
        }

        public void Commit(string tag, string message, int priority)
        {
            if(level != OFF)
            {
                if(priority >= this.level)
                {
                    if (stack.Length >= 1240000)
                        stack = "";

                    int id = Process.GetCurrentProcess().Id;

                    stack += DateTime.Now.ToString("yyyy-M-d--H:m:ss.fff") + " " 
                        + id + ":" + levelToChar(priority) + "/" + tag + ":" 
                        + message + "\n";

                    this.file.Write(stack);
                }
            }
        }
    }
}
