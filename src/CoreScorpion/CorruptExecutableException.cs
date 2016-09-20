using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    [Serializable]
    class CorruptExecutableException : Exception
    {
        public CorruptExecutableException()
        {
        }

        public CorruptExecutableException(string message)
        : base(message)
        {
        }

        public CorruptExecutableException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
