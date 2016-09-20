using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    class Environment
    {
        public const int REGISTER_MAX = 0x00a;

        /*
         * private registers for each virtual machine
         * which is used for fast data processing
         */
        public long[] registers;

        public long pc;

        //public DataStack stack;

        //public Frame frame;

        public Environment()
        {
            registers = new long[REGISTER_MAX];
            pc = 0;
            //stack = new DataStack();
            //frame = new Frame();
        }
    }
}
