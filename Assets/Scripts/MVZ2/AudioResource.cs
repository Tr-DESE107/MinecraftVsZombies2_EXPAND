using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    [CreateAssetMenu(fileName = "NewAudioMeta", menuName = "Audio Meta")]
    public class AudioResource : ScriptableObject
    {
        public NamespaceID id;
        public AudioClip[] clips;
        public int priority;
        public int maxCount;
    }
}
