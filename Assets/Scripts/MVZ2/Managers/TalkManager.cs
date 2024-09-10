using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class TalkManager : MonoBehaviour
    {
        public string TranslateText(NamespaceID textKey)
        {
            return textKey.ToString();
        }
    }
}
