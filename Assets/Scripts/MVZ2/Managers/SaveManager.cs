using UnityEngine;

namespace MVZ2
{
    public class SaveManager : MonoBehaviour
    {
        public bool IsPrologueCleared()
        {
            return false;
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
    }
}
