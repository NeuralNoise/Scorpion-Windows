using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreScorpion
{
    class Monitor
    {
        private const int MONITOR_BUSY = 0x004c;
        private const int MONITOR_FREE = 0x007d;
        public const int LOCK_INDEFINITE = 0;

        private int threadId;
        private int state;

        public Monitor()
        {
            state = MONITOR_FREE;
            threadId = -1;
        }

        public int GetThread()
        {
            return threadId;
        }

        public int State()
        {
            return state;
        }

        public bool Lockable()
        {
            return state == MONITOR_FREE &&
                threadId == -1;
        }

        public bool Lock(int spins = LOCK_INDEFINITE)
        {
            if (Globals.Threads == null || VThread.CurrentThread() == null)
                return false;

            VThread self = VThread.CurrentThread();

            if (threadId == self.Id || Globals.Threads.Length == 1)
                return true;

        wait:
            try
            {
                WaitForLock(spins);

                if (!Lockable())
                {
                    if (spins == LOCK_INDEFINITE)
                        goto wait;
                    else
                        return false;
                }

                threadId = self.Id;
                state = MONITOR_BUSY;
                return true;
            }
            catch (Exception)
            {
                if (spins == LOCK_INDEFINITE)
                    goto wait;
                else
                    return false;
            }
        }

        private void WaitForLock(int spins)
        {
            int sMaxRetries = spins;
            int retryCount = 1;

            while (!Lockable())
            {
                
                if (retryCount++ == sMaxRetries)
                {
                    if (spins == LOCK_INDEFINITE)
                    {
                        retryCount = 1;
                        continue;
                    }

                    return;
                }
            }
        }

        public void UnLock()
        {
            threadId = -1;
            state = MONITOR_FREE;
        }
    }
}
