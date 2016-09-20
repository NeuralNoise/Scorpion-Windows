using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreScorpion
{
    enum ThreadState
    {
        THREAD_INIT = 0,
        THREAD_RUNNING = 1,
        THREAD_WAIT = 2,
        THREAD_ZOMBIE = 3
    }

    enum ThreadPriority
    {
        NORMAL = 0,
    }

    class VThread
    {
        [ThreadStatic]
        public static VThread self = null;

        private const int ZOMBIE_MAX = 124;
        private const int MAIN_THREAD_ID = 1;

        public int Id { get; }

        public Monitor monitor;

        public bool dameon;

        public ThreadPriority priority;

        public ScorpionVM Machine { get; set; }

        public bool isSuspended;

        public bool isMain;

        public IO io;

        public string Name { get; }

        public Heap local;

        public ThreadState state;

        private Thread handle;

        public bool SuspendPending { get; set; }

        public VThread()
        {
        }

        public VThread(int priority, string name, int start_proc)
        {
            if(Globals.ThreadZombieCount >= ZOMBIE_MAX)
            {
                throw new Exception("There are too man zombies.");
            }

            Id = ++Globals.ThreadIdSeq;
            ChangeState(this, ThreadState.THREAD_INIT);
            monitor = new Monitor();
            dameon = false;
            this.priority = (ThreadPriority) priority;
            Machine = null;

            isSuspended = false;
            isMain = Id == MAIN_THREAD_ID;
            Name = name;
            io = null;
            local = null;
            handle = null;
        }

        public static VThread CurrentThread()
        {
            return self;
        }

        public static void ChangeState(VThread thread, ThreadState newState)
        {
            if (thread == null)
                thread = self;

            Globals.Logger.Commit("Thread", "threadid=" + thread.Id + ": (state " 
                + thread.state + " -> " + newState, Log.INFO);
            
            thread.state = newState;
        }

        private static bool CheckSupendPending()
        {
            if (self.SuspendPending == false)
                return false;
            
            Globals.Logger.Commit("Thread", "threadid=" + self.Id + 
                ": self-suspending", Log.INFO);

            SuspendSelf();

            self.SuspendPending = false;
            return true;
        }

        public static void UnsuspendThread(VThread thread)
        {
            thread.isSuspended = false;
        }

        public void Wait()
        {
            long sMaxRetries = 1000 * 1000;
            long retryCount = 0;
            
            ChangeState(self, ThreadState.THREAD_WAIT);

            while (IsSuspended())
            {
                if (retryCount++ == sMaxRetries)
                {
                }
            }

            ChangeState(self, ThreadState.THREAD_RUNNING);
            self.isSuspended = false;
        }

        public static void SuspendSelf()
        {
            self.isSuspended = true;

            Globals.Logger.Commit("Thread", "threadid=" + self.Id +
                ": self-suspending", Log.INFO);

            /*
	        * We call wait upon suspend. This function will
	        * sleep the hread most of the time. notify() or
	        * resumeAllThreads() sould be called to revive thread.
	        */
            self.Wait();

            Globals.Logger.Commit("Thread", "threadid=" + self.Id +
                ": self-reviving", Log.INFO);
        }

        public static void Notify()
        {
            VThread thread;

            Globals.Logger.Commit("Thread", "threadid=" + self.Id + ": thread notify starting.", Log.DEBUG);
            
            /*
            * We search for the next avaliable suspended thread. The next thread is 
            * picked in a "top down" manner. We first search for threads with the highest
            * proirity to be resumed. We also ignore zombie threads. Those threads 
            * should not be brought back to life!
            */
            for (int i = 0; i < Globals.Threads.Length; i++)
            {
                thread = Globals.Threads[i];

                if (thread == self || 
                    thread.state == ThreadState.THREAD_ZOMBIE
                    || thread.state == ThreadState.THREAD_RUNNING)
                    continue;

                /*
                * Resume the thread and move on
                */
                UnsuspendThread(thread);
                Globals.Logger.Commit("Thread", "threadid=" + self.Id + ": thread " + 
                    thread.Id + " waking up.", Log.DEBUG);

                return;
            }
        }

        private bool ThreadActive()
        {
            return state != ThreadState.THREAD_ZOMBIE &&
                state != ThreadState.THREAD_INIT;
        }

        public static int Start(int id)
        {
            VThread thread = GetThread(id);

            if (thread == null)
                return 1;

            if(thread.ThreadActive())
            {
                Globals.Logger.Commit("Thread", "Illegal thread state. Thread is already active!", Log.ERROR);
                return 1;
            }

            if(thread.handle != null)
            {
                thread.handle = new Thread(
                    BytecodeInterpreter.InterpreterThreadStart);
            }

            try
            {
                thread.handle.Start();
                return 0;
            }
            catch(Exception)
            {
                Globals.Logger.Commit("Thread", "Thread failed to start/restart.", Log.ERROR);
                return 1;
            }
        }

        public static int Interrupt(int id)
        {
            if(id == self.Id)
            {
                Globals.Logger.Commit("Thread", "threadid=" + self.Id + 
                    ": attempting to interrupt its-self.", Log.DEBUG);
                return 1;
            }

            VThread thread = GetThread(id);
            if(thread == null)
            {
                Globals.Logger.Commit("Thread", "threadid=" + id +
                    ": does not exist.", Log.DEBUG);
                return 1;
            }

            return ThreadInterrupt(thread);
        }

        private static int ThreadInterrupt(VThread thread)
        {
            if (thread.state == ThreadState.THREAD_RUNNING)
            {
                WaitForThreadSuspend(thread);

                if (thread.isMain)
                {

                    /*
                    * Shutdown all running threads
                    * and de-allocate al allocated
                    * memory. If we do not call join()
                    * to wait for all other threads
                    * regardless of what they are doing, we
                    * stop them.
                    */
                    ScorpionVM.Shutdown(true);
                }
                else
                {
                    try
                    {
                        thread.handle.Interrupt();
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
            }

            return -1;
        }

        public static int Join(int id)
        {
            if (id == self.Id)
            {
                Globals.Logger.Commit("Thread", "threadid=" + self.Id +
                    ": attempting to join its-self.", Log.DEBUG);
                return 1;
            }

            VThread thread = GetThread(id);
            if (thread == null)
            {
                Globals.Logger.Commit("Thread", "threadid=" + id +
                    ": does not exist.", Log.DEBUG);
                return 1;
            }

            return ThreadJoin(thread);
        }

        private static int ThreadJoin(VThread thread)
        {
            if (thread.state == ThreadState.THREAD_RUNNING)
            {
                try
                {
                    thread.handle.Join();
                }
                catch (Exception)
                {
                    return -1;
                }
            }

            return -1;
        }

        public static void ShutdownAllThreads()
        {
            if (Globals.Threads == null)
                return;

            VThread thread;

            ChangeState(self, ThreadState.THREAD_WAIT);
            SuspendAllThreads();

            /*
	        * Close all threads except the current
	        * calling thread. We want to control how we exit!
	        */
            for (int i = Globals.Threads.Length - 1; i > 0; i--)
            {
                thread = Globals.Threads[i];

                if (thread == self)
                    continue;

                ThreadInterrupt(thread);
                thread.ReleaseThread();
            }

            ChangeState(self, ThreadState.THREAD_RUNNING);
        }

        public static void PrepareThread()
        {

        }

        public void ThreadExit()
        {

        }

        public void ReleaseThread()
        {

        }

        public static void ResumeAllThreads()
        {
            VThread thread;

            Globals.Logger.Commit("Thread", "threadid=" + self.Id +
                ": ResumeAll starting.", Log.INFO);

            /*
	         * We ignore zombie threads. Those threads should not be 
	         * brought back to life!
	         */
            for (int i = 0; i < Globals.Threads.Length; i++)
            {
                thread = Globals.Threads[i];

                if (thread == self || thread.state == ThreadState.THREAD_ZOMBIE)
                    continue;

                UnsuspendThread(thread);
                ChangeState(thread, ThreadState.THREAD_RUNNING);
            }
         }

        public static void SuspendAllThreads()
        {
            VThread thread;

            /*
	         * Wait for everybody in THREAD_RUNNING state to stop.  Other states
	         * indicate the code is either running natively or sleeping quietly.
	         * Any attempt to transition back to THREAD_RUNNING will cause a check
	         * for suspension, so it should be impossible for anything to execute
	         * interpreted code or modify objects (assuming native code plays nicely).
	         * tecnically the thread will still be running, but each thread will call wait 
	         * immediatley once it realizes it has been suspended and no interpreted code should 
	         * run.
	         *
	         * It's also okay if the thread transitions to a non-RUNNING state.
	         *
	         * We ignore all zombie threads to avoid possible weird wait errors.
	         */
            for (int i = 0; i < Globals.Threads.Length; i++)
            {
                thread = Globals.Threads[i];

                if (thread == self || thread.state == ThreadState.THREAD_ZOMBIE)
                    continue;

                if(thread.state == ThreadState.THREAD_RUNNING)
                {
                    WaitForThreadSuspend(thread);
                    Globals.Logger.Commit("Thread", "threadid=" + self.Id +
                        ": threadid=" + thread.Id + " state=" + thread.state +
                        " isSusp=" + thread.isSuspended + ".", Log.INFO);
                }
            }
        }

        public static void SuspendThread(VThread thread)
        {
            thread.SuspendPending = true;
        }

        private static void WaitForThreadSuspend(VThread thread)
        {
            const int sMaxRetries = 10000000;
            const int sMaxSpinCount = 25;
            
            int spinCount = 0;
            int retryCount = 0;

            SuspendThread(thread);
            while (thread.state == ThreadState.THREAD_RUNNING && !thread.isSuspended)
            {

                if (retryCount++ == sMaxRetries)
                {
                    if(++spinCount >= sMaxSpinCount)
                    {
                        Globals.Logger.Commit("Thread", "threadid=" + self.Id +
                            ": stuck on thread threadid=" + thread.Id + " giving up.", Log.INFO);
                        return;
                    }
                }
            }
        }

        private static bool IsSuspended()
        {
            return (self.isSuspended == true &&
                self.state != ThreadState.THREAD_RUNNING)
                || self.state != ThreadState.THREAD_RUNNING;
        }

        public static void LinkThread(VThread thread)
        {
            Globals.ThreadMonitor.Lock();

            int listCount = Globals.Threads == null ? 
                1 : Globals.Threads.Length+1;
            int iter = 0;

            VThread[] lst = new VThread[listCount];
            for(int i = 0; i < Globals.Threads.Length; i++)
            {
                lst[iter++] = Globals.Threads[i];
            }

            Globals.Threads = null;
            lst[iter] = thread;

            Globals.Threads = lst;
            Globals.ThreadMonitor.UnLock();
        }

        private static VThread GetThread(int id)
        {
            if (Globals.Threads == null)
                return null;

            for(int i = 0; i < Globals.Threads.Length; i++)
            {
                if (Globals.Threads[i].Id == id)
                    return Globals.Threads[i];
            }

            return null;
        }

        public static void UnlinkThread(int id)
        {
            Globals.ThreadMonitor.Lock();
            if(Globals.Threads == null || GetThread(id) == null)
            {
                Globals.ThreadMonitor.UnLock();
                return;
            }
            
            VThread[] lst;
            if(Globals.Threads.Length == 1)
            {
                Globals.Threads = null;
                Globals.ThreadMonitor.UnLock();
                return;
            }
            else
            {
                lst = new VThread[Globals.Threads.Length - 1];
            }

            int iter = 0;
            for (int i = 0; i < Globals.Threads.Length; i++)
            {
                if (Globals.Threads[i].Id != id)
                {
                    lst[iter++] = Globals.Threads[i];
                }
            }

            Globals.Threads = null;
            Globals.Threads = lst;
            Globals.ThreadMonitor.UnLock();
        }
    }
}
