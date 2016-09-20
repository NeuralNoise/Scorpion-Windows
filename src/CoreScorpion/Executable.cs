using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    struct EHeader
    {
        public int    type;
        public int    machine;
        public int    version;
        public int    debug;
        public int    log;
        public int    logLevel;
        public string logFile;
        public string appId;
        public long   size;
        public string name;
        public long   addressBase;
        public long   stringBase;
        public long   localStorageBase;
        public long   functionBase;
        public long   classBase;
        public long   entry;
    };

    struct EObject
    {
        /*
         * unique Address of object in memory 
         */
        public long id;

        /*
         * Name of our object
         */
        public string name;

        /*
         * Refrence ID to a class. If the specified object
         * is of class type it will contain a refrence to a class,
         * otherwise the refrence will be -1
         */
        public long classRefrence;
    };

    struct EClass
    {
        /*
         * Refrence to this classes super class
         */
        public long superClassRefrence;

        /*
         * unique Address of object in memory 
         */
        public long id;

        /*
         * Full name details about class
         */
        public string descriptor;

        /*
         * The list of objects that this class contains
         */
        public EObject[] objects;
    };

    struct EString
    {
        /*
         * The unique refrence id to the string
         */
        public long id;

        /*
         * The string its-self
         */
        public string data;
    };

    class ByteCode
    {
        public byte opcode;
        public double[] args;

        public ByteCode(byte opcode, params double[] args)
        {
            this.opcode = opcode;
            this.args = args.Length == 0 ? null : args;
        }
    };

    class Executable
    {
        public string fileName;
        public ByteCode[] bytecode;
        public string[] args;
        public EHeader header;

        public EClass[] cObjects;
        public EObject[] objects;
        public EObject[] lObjects;
        public EString[] srings;

        public Executable()
        {
            fileName = "";
            bytecode = null;
            args = null;
        }

        override public string ToString()
        {
            return string.Format("############ Executable Header ############\n"
                + "file:               {0}\n"
                + "magic:              {1}\n"
                + "type:               {2}\n"
		        + "machine:            {3}\n"
		        + "version:            {4}\n"
		        + "debug:              {5}\n"
		        + "log:                {6}\n"
		        + "log level:          {7}\n"
		        + "log file:           {8}\n"
		        + "application id:     {9}\n"
		        + "image size:         {10}\n"
		        + "name:               {11}\n"
		        + "address base:       {12}\n"
		        + "string refs:        {13}\n"
		        + "classes:            {14}\n"
		        + "TLS address base:   {15}\n"
		        + "functions:          {16}\n"
                + "entry:              {17}\n"
                + "----\n", 
                fileName, "0x65 0x7f 0x2b 0x5a",
                header.type, header.machine, header.version,
                header.debug, header.log, header.logLevel,
                header.logFile, header.appId, header.size,
                header.name, header.addressBase, header.stringBase,
                header.classBase, header.localStorageBase, 
                header.functionBase, header.entry);
        }
    }
}
