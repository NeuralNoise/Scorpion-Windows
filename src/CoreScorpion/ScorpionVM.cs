using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    class ScorpionVM
    {
        /*
         * Unique id of virtual machine
         */
        int serialNumber;

        /*
         * Flag to notify the virtual machine when 
         * an event has occured
         */
        int signal;

        /*
         * Ececution environment for the bytecode interpreter
         */
        Environment env;

        public static bool Startup(string[] pargs)
        {
            Globals.Logger.Commit("VirtualMachine", "VM init Args (" + 
                pargs.Length + "):", Log.DEBUG);

            for (int i = 0; i < pargs.Length; i++)
            {
                Globals.Logger.Commit("VirtualMachine", " " + i + ": '" +
                    pargs[i] + "'", Log.DEBUG);
            }



            return true;
        }

        public static void Shutdown(bool exit)
        {

        }

        public void ShutdownSelf()
        {

        }

        public static int GetSerialNumber()
        {
            return Globals.VmSerialIdSeq++;
        }
    }
}
