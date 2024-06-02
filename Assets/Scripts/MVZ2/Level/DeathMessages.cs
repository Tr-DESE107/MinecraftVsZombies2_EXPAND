using System.Collections.Generic;
using MukioI18n;
using MVZ2.GameContent;
using PVZEngine;

namespace MVZ2.Level
{
    public static class DeathMessages
    {
        public const string CONTEXT = "死亡信息";
        [TranslateMsg("死亡信息-未知", CONTEXT)]
        public const string UNKNOWN = "你死了！";
        [TranslateMsg("死亡信息-僵尸", CONTEXT)]
        public const string ZOMBIE = "脑子是好的，可惜不是每个人都有。";

        public static string GetByEntityID(NamespaceID id)
        {
            if (entityDeathMessageDict.TryGetValue(id, out var message))
                return message;
            return UNKNOWN;
        }

        private static readonly Dictionary<NamespaceID, string> entityDeathMessageDict = new Dictionary<NamespaceID, string>()
        {
            { EnemyID.zombie, ZOMBIE },
            { EnemyID.leatherCappedZombie, ZOMBIE },
            { EnemyID.ironHelmettedZombie, ZOMBIE },
            { EnemyID.flagZombie, ZOMBIE },

        };
    }
}
