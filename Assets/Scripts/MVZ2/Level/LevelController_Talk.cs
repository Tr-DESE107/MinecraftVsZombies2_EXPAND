using System;
using MVZ2.Talk;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using MVZ2Logic.Level;
using MVZ2Logic.Talk;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法

        #region 对话
        public void StartTalk(NamespaceID groupId, int section, float delay = 0)
        {
            talkController.StartTalk(groupId, section, delay);
        }
        public void TryStartTalk(NamespaceID groupId, int section, float delay = 0, Action<bool> onFinished = null)
        {
            talkController.TryStartTalk(groupId, section, delay, onFinished);
        }
        #endregion

        #endregion

        #region 私有方法

        #region 事件回调

        private void UI_OnTalkActionCallback(string cmd, string[] parameters)
        {
            Global.Game.RunCallbackFiltered(VanillaCallbacks.TALK_ACTION, cmd, talkSystem, cmd, parameters);
        }
        private void UI_OnTalkEndCallback()
        {
            if (level.IsCleared)
            {
                StartExitLevelTransition(0);
            }
            else
            {
                level.BeginLevel();
            }
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
