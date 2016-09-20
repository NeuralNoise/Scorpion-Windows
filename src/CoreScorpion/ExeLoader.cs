using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    static class ExeLoader
    {

        /*
         * Constants
         */
        private const int HEADER_INDENT = 16;
        private const int HEADER_END = 0x3f;
        private const int HEADER_SIZE = 16;
        private const int TYPE_OBJECT_FILE = 1;
        private const int FILE_VERSION = 1;
        private const int CLASS_FLAG = 0xa7;
        private const int OBJECT_FLAG = 0x16;
        private const int STRING_FLAG = 0x19;
        private const int METHOD_FLAG = 0x33;
        private const int IMAGE_FLAG = 0x6d;
        private const int IMAGE_END = 0x12;

        private static string ReadString(StreamBuffer buf)
        {
            char c = ' ';
            string str = "";

            while(c != 0)
            {
                c = buf.ReadChar();

                if (c != 0)
                    str += c;
            }

            return str;
        }

        private static EClass ReadClass(StreamBuffer buffer)
        {
            EClass klass = new EClass();

            if(buffer.Check((char)CLASS_FLAG))
            {
                klass.superClassRefrence = long.Parse(ReadString(buffer));
                klass.id = long.Parse(ReadString(buffer));
                klass.descriptor = ReadString(buffer);
                klass.objects = null;

                int size = int.Parse(ReadString(buffer));
                if(size > 0)
                {
                    klass.objects = new EObject[size];
                    for(int i = 0; i < size; i++)
                    {
                        klass.objects[i] = ReadObject(buffer);
                    }
                }

                return klass;
            }

            throw new CorruptExecutableException("corrupt image, could not read class.");
        }

        private static EObject ReadObject(StreamBuffer buffer)
        {
            EObject Object = new EObject();

            if (buffer.Check((char)OBJECT_FLAG))
            {
                Object.classRefrence = long.Parse(ReadString(buffer));
                Object.id = long.Parse(ReadString(buffer));
                Object.name = ReadString(buffer);
                
                return Object;
            }

            throw new CorruptExecutableException("corrupt image, could not read object.");
        }

        private static Method ReadFunction(StreamBuffer buffer)
        {
            Method function = new Method();

            if (buffer.Check((char)METHOD_FLAG))
            {
                function.classRefrence = long.Parse(ReadString(buffer));
                function.id = long.Parse(ReadString(buffer));
                function.name = ReadString(buffer);
                function.entry = long.Parse(ReadString(buffer));
                function.file = ReadString(buffer);
                function.offset = 0;

                return function;
            }

            throw new CorruptExecutableException("corrupt image, could not read function.");
        }

        private static EString ReadStringRefrence(StreamBuffer buffer)
        {
            EString String = new EString();

            if (buffer.Check((char)STRING_FLAG))
            {
                String.id = long.Parse(ReadString(buffer));
                String.data = ReadString(buffer);

                return String;
            }

            throw new CorruptExecutableException("corrupt image, could not read string refrence.");
        }

        public static bool Load(string[] args)
        {
            try
            {
                string path = args[0];
                string[] pArgs = null;
                if(args.Length > 1)
                {
                    pArgs = new string[args.Length - 1];
                    for(int i = 1; i < args.Length; i++)
                    {
                        pArgs[i - 1] = args[i];
                    }
                }

                Globals.Exe.args = pArgs;
                File file = new File(path);
                file.Read();

                /* 
                 * File signature check 
                 */
                if (!file.Check((char)0x7f, 'E', 'O', 'F'))
                    ConsoleHelper.Error("invalid character found in file at {0}", 
                        file.ReadChar());

                file.Advance(HEADER_INDENT);
                /*
                 * File format check (magic number)
                 */
                if (!file.Check((char)0x65, (char)0x7f, 
                    (char)0x2b, (char)0x5a))
                    ConsoleHelper.Error("invalid magic numer at {0}",
                        file.ReadChar());

                int id;
                int tracker = 0; // tracker for how many header entries have been read

                do
                {
                    id = file.ReadChar();

                    switch(id)
                    {
                        case 0x0:
                            break;
                        case 0x01:
                            int type = file.ReadChar();

                            if (type == TYPE_OBJECT_FILE)
                                Globals.Exe.header.type = type;
                            else
                                ConsoleHelper.Error("The type of file provided cannot be processed `" + type + "`");
                            break;
                        case 0x02:
                            string target = ReadString(file.GetBuffer());
                            Globals.Exe.header.machine = int.Parse(target);
                            break;

                        case 0x03:
                            int version = file.ReadChar();
                            if (version != FILE_VERSION)
                                ConsoleHelper.Error("The verson of exe file provided cannot be processed `" + version + "`");
                            else
                                Globals.Exe.header.version = version;
                            break;
                        case 0x04:
                            Globals.Exe.header.debug = file.ReadChar();
                            break;
                        case 0x05:
                            Globals.Exe.header.log = file.ReadChar();
                            break;
                        case 0x06:
                            string level = ReadString(file.GetBuffer());
                            Globals.Exe.header.logLevel = int.Parse(level);
                            break;
                        case 0x07:
                            Globals.Exe.header.logFile = ReadString(file.GetBuffer());
                            break;
                        case 0x08:
                            Globals.Exe.header.appId = ReadString(file.GetBuffer());
                            break;
                        case 0x09:
                            long size = long.Parse(ReadString(file.GetBuffer()));
                            if (size > 0)
                                Globals.Exe.header.size = size;
                            else
                                ConsoleHelper.Error("image section is empty.");
                            break;
                        case 0x11:
                            Globals.Exe.header.name = ReadString(file.GetBuffer());
                            break;
                        case 0x12:
                            string addressBase = ReadString(file.GetBuffer());
                            Globals.Exe.header.addressBase = long.Parse(addressBase);
                            break;
                        case 0x13:
                            string stringBase = ReadString(file.GetBuffer());
                            Globals.Exe.header.stringBase = int.Parse(stringBase);
                            break;
                        case 0x14:
                            string locaStorageBase = ReadString(file.GetBuffer());
                            Globals.Exe.header.localStorageBase = int.Parse(locaStorageBase);
                            break;
                        case 0x15:
                            long functionBase = long.Parse(ReadString(file.GetBuffer()));
                            if (functionBase > 0)
                                Globals.Exe.header.functionBase = functionBase;
                            else
                                ConsoleHelper.Error("image dosent conain any functions.");
                            break;
                        case 0x16:
                            string entryPoint = ReadString(file.GetBuffer());
                            Globals.Exe.header.entry = long.Parse(entryPoint);
                            break;
                        case 0x17:
                            string classBase = ReadString(file.GetBuffer());
                            Globals.Exe.header.classBase = long.Parse(classBase);
                            break;
                        case HEADER_END:
                            break;
                        default:
                            return false;
                    }

                    if(id == HEADER_END)
                    {
                        tracker--;
                        if (tracker != HEADER_SIZE)
                            return false;

                        if (Globals.Exe.header.machine > Application.Constants.MachineVersion)
                            return false;

                        break;
                    }
                } while (true);
                
                if (!file.Check((char)0x47, (char)0x1b, (char)0x7d))
                    ConsoleHelper.Error("image section is corrupted.");

                string image = file.ReadAllChars();
                file.Release();

                if (image.Length > 0)
                {
                    StreamBuffer buffer = new StreamBuffer(Zip.DecompressString(image));

                    if(Globals.Exe.header.classBase > 0)
                    {
                        Globals.Exe.cObjects = new EClass[Globals.Exe.header.classBase];
                        for (long i = 0; i < Globals.Exe.header.classBase; i++)
                        {
                            Globals.Exe.cObjects[i] = ReadClass(buffer);
                        }
                    }

                    if (Globals.Exe.header.addressBase > 0)
                    {
                        Globals.Exe.objects = new EObject[Globals.Exe.header.addressBase];
                        for (long i = 0; i < Globals.Exe.header.addressBase; i++)
                        {
                            Globals.Exe.objects[i] = ReadObject(buffer);
                        }
                    }

                    if (Globals.Exe.header.localStorageBase > 0)
                    {
                        Globals.Exe.lObjects = new EObject[Globals.Exe.header.localStorageBase];
                        for (long i = 0; i < Globals.Exe.header.localStorageBase; i++)
                        {
                            Globals.Exe.lObjects[i] = ReadObject(buffer);
                        }
                    }

                    if(Globals.Exe.header.stringBase > 0)
                    {
                        Globals.Exe.srings = new EString[Globals.Exe.header.stringBase];
                        for(long i = 0; i < Globals.Exe.header.stringBase; i++)
                        {
                            Globals.Exe.srings[i] = ReadStringRefrence(buffer);
                        }
                    }

                    if(Globals.Exe.header.functionBase > 0)
                    {
                        Globals.functions = new Method[Globals.Exe.header.functionBase];
                        for(long i = 0; i < Globals.Exe.header.functionBase; i++)
                        {
                            Globals.functions[i] = ReadFunction(buffer);
                        }
                    }

                    Globals.Exe.bytecode = new ByteCode[Globals.Exe.header.size];
                    if(!buffer.Check((char)IMAGE_FLAG))
                    {
                        ConsoleHelper.Error("image section could not be processed.");
                    }

                    int flag, opcode;
                    long index = 0;

                    for (;;)
                    {
                        flag = buffer.ReadChar();

                        switch(flag)
                        {
                            case 0x0:
                                break;
                            case 0x01:
                                flag = buffer.ReadChar();
                                opcode = buffer.ReadChar();

                                if (opcode < 0 || opcode > OpcodeConstants.MaxOpcode)
                                    ConsoleHelper.Error("invalid opcode: " + opcode);

                                switch (flag)
                                {
                                    case 0:
                                        Globals.Exe.bytecode[index++] = new ByteCode((byte)opcode);
                                        break;
                                    case 1:
                                        Globals.Exe.bytecode[index++] = new ByteCode((byte)opcode, 
                                            double.Parse(ReadString(buffer)));
                                        break;
                                    case 2:
                                        Globals.Exe.bytecode[index++] = new ByteCode((byte)opcode, 
                                            double.Parse(ReadString(buffer)), double.Parse(ReadString(buffer)));
                                        break;
                                    case 3:
                                        Globals.Exe.bytecode[index++] = new ByteCode((byte)opcode, 
                                            double.Parse(ReadString(buffer)), double.Parse(ReadString(buffer)), 
                                            double.Parse(ReadString(buffer)));
                                        break;
                                    default:
                                        ConsoleHelper.Error("invalid opcode flag: " + opcode);
                                        break;
                                }
                                break;
                            case IMAGE_END:
                                break;
                            default:
                                ConsoleHelper.Error("image section could not be processed.");
                                break;
                        }

                        if(flag == IMAGE_END)
                        {
                            if (index == Globals.Exe.header.size)
                                return false;

                            break;
                        }
                    }

                    return true;
                }


                ConsoleHelper.Error("image section is corrupted.");
                return false;
            }
            catch(CorruptExecutableException err)
            {
                ConsoleHelper.Error(err.Message);
                return false;
            }
            catch(IndexOutOfRangeException)
            {
                ConsoleHelper.Error("image section is too short.");
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
