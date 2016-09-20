using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    class File
    {
        private string fileName;
        private StreamBuffer buffer;
        private Monitor monitor;

        public File(string fileName)
        {
            monitor = new Monitor();
            this.fileName = fileName;
            buffer = new StreamBuffer();
        }
        public File()
        {
            this.fileName = "";
            buffer = new StreamBuffer();
        }

        public void SetFileName(string fileName)
        {
            this.fileName = fileName;
        }

        public int Size()
        {
            return buffer.Size();
        }

        public char At(int index)
        {
            return buffer.At(index);
        }

        public char ReadChar()
        {
            return buffer.ReadChar();
        }

        public StreamBuffer GetBuffer()
        {
            return buffer;
        }

        public string ReadChars(int chars)
        {
            return buffer.ReadChars(chars);

        }

        public string ReadAllChars()
        {
            return buffer.ReadAllChars();
        }

        public void Advance(int chars)
        {
            buffer.Advance(chars);
        }

        public bool Check(params char[] chars)
        {
            return buffer.Check(chars);
        }

        public void Read()
        {
            monitor.Lock();
            buffer.SetStream(System.IO.File.ReadAllText(fileName));
            monitor.UnLock();
        }

        public void Release()
        {
            buffer.Release();
            fileName = "";
        }

        public bool Write(string data)
        {
            return Write(data.ToArray());
        }

        public bool Write(char[] data)
        {
            monitor.Lock();
            try
            {
                System.IO.File.WriteAllText(fileName, new string(data));
                monitor.UnLock();
                return true;
            }
            catch(Exception)
            {
                monitor.UnLock();
                return false;
            }
        }
    }
}
