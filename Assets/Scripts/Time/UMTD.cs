//
// Developed by TerraStudios.
// This script is covered by a Mutual Non-Disclosure Agreement and is Confidential.
// Destroy the file immediately if you are not one of the parties involved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Unity Main Thread Dispatcher (UMTD) class
    /// Used for calling code on the main thread from other threads.
    /// Third-party script.
    /// </summary>
    public class UMTD : MonoBehaviour
    {
        private readonly Queue<Action> executionQueue = new Queue<Action>();

        public void Update()
        {
            lock (executionQueue)
            {
                while (executionQueue.Count > 0)
                {
                    executionQueue.Dequeue().Invoke();
                }
            }
        }

        public void Enqueue(IEnumerator action)
        {
            lock (executionQueue)
            {
                executionQueue.Enqueue(() =>
                {
                    StartCoroutine(action);
                });
            }
        }

        public void Enqueue(Action action)
        {
            Enqueue(ActionWrapper(action));
        }

        public Task EnqueueAsync(Action action)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            void WrappedAction()
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }

            Enqueue(ActionWrapper(WrappedAction));
            return tcs.Task;
        }

        private IEnumerator ActionWrapper(Action a)
        {
            a();
            yield return null;
        }
    }
}
