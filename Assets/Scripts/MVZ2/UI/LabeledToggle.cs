using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class LabeledToggle : MonoBehaviour
    {
        public TextMeshProUGUI Text => text;
        public Toggle Toggle => toggle;
        [SerializeField]
        private TextMeshProUGUI text;
        [SerializeField]
        private Toggle toggle;
    }
}
