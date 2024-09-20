using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MVZ2.GameContent;
using MVZ2.Level.UI;
using MVZ2.Talk;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Game;
using PVZEngine.Level;
using PVZEngine.Serialization;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    using LevelEngine = PVZEngine.Level.LevelEngine;
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法

        #region 对话
        public void StartTalk(NamespaceID groupId, int section, float delay = 0)
        {
            talkController.StartTalk(groupId, section, delay);
        }
        #endregion

        #endregion

        #region 私有方法

        #region 事件回调

        private void UI_OnTalkActionCallback(string cmd, string[] parameters)
        {
            BuiltinCallbacks.TalkAction.RunFiltered(cmd, talkController, cmd, parameters);
        }
        private void UI_OnTalkEndCallback(NamespaceID mode)
        {
            var talkEndDefinition = Game.GetTalkEndDefinition(mode);
            if (talkEndDefinition != null)
            {
                talkEndDefinition.Execute(level);
            }
            else
            {
                if (level.IsCleared)
                {
                    StartCoroutine(ExitLevelTransition());
                }
                else
                {
                    level.BeginLevel(LevelTransitions.DEFAULT);
                }
            }
        }

        #endregion

        #endregion

        #region 属性字段
        [SerializeField]
        private TalkController talkController;
        #endregion
    }
}
