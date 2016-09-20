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

        /*
         * Our full executable file
         */
        public static Executable Exe;

        /*
         * The list of global functions thatcan be called
         */
        public static Method[] functions;

        /*
         * Thread list for virtual machine
         */
        public static VThread[] Threads;

        /*
         * Unique counter for getting unique virtual machine id's
         */
        public static int VmSerialIdSeq = 10228;

        /*
         * Unique Id Sequence for creating threads
         */
        public static int ThreadIdSeq = -1;

        /*
         * Number of dead threads still used in memory
         */
        public static int ThreadZombieCount = 0;

        /*
         * Monitor for thread manipulation
         */
        public static Monitor ThreadMonitor;

        /*
         * Monitor for critical sections of code
         */
        public static Monitor CriticalMonitor;

        public static void Init()
        {
            Logger = new Log("");
            Exe = new Executable();
            functions = null;
            Threads = null;
            ThreadMonitor = new Monitor();
            CriticalMonitor = new Monitor();
        }
    }
}
