using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    static class Globals
    {
        /** 
        * Flag for native assertions 
        */
        public static bool AssertEnabled = true;

        /**
        * Flag for what mode the virtual machine will start in 
        */
        public static bool DebugEnabled = false;

        /**
        * Flag for showing detailed information about your executable file
        */
        public static bool ShowExe = false;

        /*
        * Application logger for the virtual machine and application
        */
        public static Log Logger;

        public static void Init()
        {
            Logger = new Log();
        }
    }
}
