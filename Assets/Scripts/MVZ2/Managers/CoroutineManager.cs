using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace MVZ2.Managers
{
    public class CoroutineManager : MonoBehaviour
    {
        public Task ToTask(Coroutine coroutine)
        {
            var tcs = new TaskCompletionSource<object>();
            StartCoroutine(routine());
            IEnumerator routine()
            {
                yield return coroutine;
                tcs.SetResult(null);
            }
            return tcs.Task;
        }
        public Task ToTask(IEnumerator enumerator)
        {
            var tcs = new TaskCompletionSource<object>();
            StartCoroutine(routine());
            IEnumerator routine()
            {
                yield return enumerator;
                tcs.SetResult(null);
            }
            return tcs.Task;
        }
        public Coroutine ToCoroutine(Task task)
        {
            return StartCoroutine(ToCoroutineFunc(task));
        }
        public IEnumerator ToCoroutineFunc(Task task)
        {
            while (!task.IsCompleted)
                yield return null;
        }
        public Task DelaySeconds(float seconds)
        {
            var tcs = new TaskCompletionSource<object>();
            StartCoroutine(routine());
            IEnumerator routine()
            {
                yield return new WaitForSeconds(seconds);
                tcs.SetResult(null);
            }
            return tcs.Task;
        }
    }
}
