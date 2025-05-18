using System;
using MVZ2.Talk;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using MVZ2Logic.Talk;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法

        #region 对话
        public bool CanStartTalk(NamespaceID groupId, int section)
        {
            return talkController.CanStartTalk(groupId, section);
        }
        public void StartTalk(NamespaceID groupId, int section, float delay = 0, Action onEnd = null)
        {
            talkController.StartTalk(groupId, section, delay, onEnd);
        }
        public bool WillSkipTalk(NamespaceID groupId, int section)
        {
            return talkController.WillSkipTalk(groupId, section);
        }
        public void SkipTalk(NamespaceID groupId, int section, Action onSkip = null)
        {
            talkController.SkipTalk(groupId, section, onSkip);
        }
        #endregion

        #endregion

        #region 私有方法

        #region 事件回调

        private void UI_OnTalkActionCallback(string cmd, string[] parameters)
        {
            Global.Game.RunCallbackFiltered(VanillaCallbacks.TALK_ACTION, new VanillaCallbacks.TalkActionParams(talkSystem, cmd, parameters), cmd);
        }
        #endregion

        #endregion

        #region 属性字段
        [SerializeField]
        private TalkController talkController;
        private ITalkSystem talkSystem;
        #endregion
    }
}
