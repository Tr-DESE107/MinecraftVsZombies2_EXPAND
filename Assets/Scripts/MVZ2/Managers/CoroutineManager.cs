using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace MVZ2.Managers
{
    public class CoroutineManager : MonoBehaviour
    {
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
    }
}
