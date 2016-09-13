using System;
using System.Collections.Generic;
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

        public int Level;

        public Log()
        {
        }
    }
}
