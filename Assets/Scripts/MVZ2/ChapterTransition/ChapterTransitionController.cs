using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.GameContent;
using MVZ2.Games;
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
            var info = transitionInfos.FirstOrDefault(i => i.id == id);
            if (info == null)
                return;
            animator.SetBool("WillRotate", !info.doNotRotate);
            ui.SetWheelRootRotation(info.angle);
            ui.SetTitleSprite(Main.LanguageManager.GetSprite(info.sprite));
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
        [SerializeField]
        private ChapterTransitionInfo[] transitionInfos;
    }
    [Serializable]
    public class ChapterTransitionInfo
    {
        public NamespaceID id;
        public float angle;
        public bool doNotRotate;
        public Sprite sprite;
    }
}
