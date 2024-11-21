using System.Collections;
using System.Threading.Tasks;

namespace MVZ2
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
