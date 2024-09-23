using UnityEngine;

namespace MVZ2.ChapterTransition
{
    [ExecuteAlways]
    public class ChapterTransitionController : MonoBehaviour
    {
        private void Update()
        {
            ui.SetTitleBlur(titleBlur);
        }
        [SerializeField]
        private ChapterTransition ui;
        [SerializeField]
        [Range(0, 1)]
        private float titleBlur;
    }
}
