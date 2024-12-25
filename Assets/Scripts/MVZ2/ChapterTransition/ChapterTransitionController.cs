using System;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Managers;
using PVZEngine;
using UnityEngine;

namespace MVZ2.ChapterTransition
{
    public class ChapterTransitionController : MonoBehaviour
    {
        public void Display(NamespaceID id)
        {
            gameObject.SetActive(true);
            var meta = Main.ResourceManager.GetChapterTransitionMeta(id);
            if (meta == null)
                return;
            animator.SetBool("WillRotate", !meta.NoRotate);
            ui.SetWheelRootRotation(meta.Angle);
            ui.SetTitleSprite(Main.GetFinalSprite(meta.TextSprite));
        }
        public async Task DisplayAsync(NamespaceID id)
        {
            tcs = new TaskCompletionSource<bool>();
            Display(id);
            await tcs.Task;
            tcs = null;
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void CallEnd()
        {
            tcs?.SetResult(true);
        }
        private TaskCompletionSource<bool> tcs;
        private MainManager Main => MainManager.Instance;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private ChapterTransition ui;
    }
}
