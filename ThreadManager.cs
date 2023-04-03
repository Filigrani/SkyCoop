using System;
using System.Collections.Generic;
using System.Text;
using SkyCoop;
using Il2Cpp;

namespace GameServer
{
    class ThreadManager
    {
        public static readonly List<Action> executeOnMainThread = new List<Action>();
        public static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        public static bool actionToExecuteOnMainThread = false;

        public static void Log(string TXT)
        {
#if (!DEDICATED)
            Console.WriteLine(TXT);
#else
            Logger.Log(TXT);
#endif
        }

        /// <summary>Sets an action to be executed on the main thread.</summary>
        /// <param name="_action">The action to be executed on the main thread.</param>
        public static void ExecuteOnMainThread(Action _action)
        {
            if (_action == null)
            {
                Log("No action to execute on main thread!");
                return;
            }

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(_action);
                actionToExecuteOnMainThread = true;
            }
        }

        /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
        public static void UpdateMain()
        {
            if (actionToExecuteOnMainThread)
            {
                executeCopiedOnMainThread.Clear();
                lock (executeOnMainThread)
                {
                    executeCopiedOnMainThread.AddRange(executeOnMainThread);
                    executeOnMainThread.Clear();
                    actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
                {
                    executeCopiedOnMainThread[i]();
                }
            }
        }
    }
}