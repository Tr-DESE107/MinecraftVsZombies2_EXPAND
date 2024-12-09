using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace MVZ2Logic
{
    public static class CoroutineHelper
    {
        public static IEnumerator ToCoroutineFunc(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
    }
}
