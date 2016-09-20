using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreScorpion
{
    class StreamBuffer
    {
        private char[] buffer;
        private int index;

        public StreamBuffer(string buf)
        {
            SetStream(buf);
        }

        public void SetStream(string buf)
        {
            buffer = new char[buf.Length];
            for (int i = 0; i < buf.Length; i++)
            {
                buffer[i] = buf.ElementAt(i);
            }

            index = 0;
        }

        public char At(int index)
        {
            return buffer[index];
        }

        public char ReadChar()
        {
            return this.buffer[index++];
        }

        public string ReadChars(int chars)
        {
            string chrs = "";
            for (int i = 0; i < chars; i++)
            {
                chrs += this.buffer[index++];
            }

            return chrs;
        }

        public string ReadAllChars()
        {

            string chrs = "";
            for (int i = 0; i < this.buffer.Length; i++)
            {
                chrs += this.buffer[index++];
            }

            return chrs;
        }

        public void Advance(int chars)
        {
            index += chars;
        }

        public bool Check(params char[] chars)
        {
            if (chars.Length == 0)
                return false;

            for (int i = 0; i < chars.Length; i++)
            {
                if (this.buffer[index++] != chars[i])
                    return false;
            }

            return true;
        }

        public int Size()
        {
            return buffer == null ? -1 : buffer.Length;
        }

        public StreamBuffer()
        {
            index = 0;
            buffer = null;
        }

        public void Release()
        {
            buffer = null;
            index = 0;
        }
    }
}
