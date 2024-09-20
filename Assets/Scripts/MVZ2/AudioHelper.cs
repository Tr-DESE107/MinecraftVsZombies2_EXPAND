using UnityEngine;

namespace MVZ2
{
    public class AudioHelper
    {
        public static float PercentageToDbA(float p)
        {
            if (p == 0)
                return -80;
            return 20 * Mathf.Log10(p);
        }
    }
}
