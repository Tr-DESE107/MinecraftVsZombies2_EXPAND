using System.Collections.Generic;
using UnityEngine;

namespace MVZ2.Supporters
{
    [SerializeField]
    class SponsorInfos
    {
        public SponsorInfos(SponsorItem[] items)
        {
            List<SponsorSaveItem> saveItems = new List<SponsorSaveItem>();
            foreach (var sponsor in items)
            {
                var planSaveItems = new List<SponsorPlanSaveItem>();
                foreach (var plan in sponsor.Plans)
                {
                    if (SponsorPlans.TryGetPlanIDByName(plan.Name, out var id))
                    {
                        planSaveItems.Add(new SponsorPlanSaveItem()
                        {
                            rank = id.rank,
                            rankType = id.type,
                        });
                    }
                }
                if (planSaveItems.Count > 0)
                {
                    saveItems.Add(new SponsorSaveItem()
                    {
                        name = sponsor.User.Name,
                        plans = planSaveItems.ToArray()
                    });
                }
            }
            sponsors = saveItems.ToArray();
        }
        public long lastUpdateTime;
        public SponsorSaveItem[] sponsors;
    }
    [SerializeField]
    class SponsorSaveItem
    {
        public string name;
        public SponsorPlanSaveItem[] plans;
    }
    [SerializeField]
    class SponsorPlanSaveItem
    {
        public int rank;
        public int rankType;
    }
    public static class SponsorPlans
    {
        public static class Furnace
        {
            public const int TYPE = 1;
            public const int FURNACE = 15;
            public const int GUNPOWDER_BARREL = 30;
            public const int BLAST_FURNACE = 50;
        }
        public static class Sensor
        {
            public const int TYPE = 2;
            public const int MOONLIGHT_SENSOR = 10;
        }

        public static bool TryGetPlanIDByName(string name, out (int type, int rank) id)
        {
            return planMap.TryGetValue(name, out id);
        }

        private static readonly Dictionary<string, (int type, int rank)> planMap = new Dictionary<string, (int type, int rank)>()
        {
            { "月光传感器", (SponsorPlans.Sensor.TYPE, SponsorPlans.Sensor.MOONLIGHT_SENSOR) },
            { "熔炉", (SponsorPlans.Furnace.TYPE, SponsorPlans.Furnace.FURNACE) },
            { "火药桶", (SponsorPlans.Furnace.TYPE, SponsorPlans.Furnace.GUNPOWDER_BARREL) },
            { "高炉",(SponsorPlans.Furnace.TYPE, SponsorPlans.Furnace.BLAST_FURNACE) },
        };
    }
}
