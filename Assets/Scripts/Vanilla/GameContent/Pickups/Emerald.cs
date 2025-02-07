using MVZ2.Vanilla.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.emerald)]
    public class Emerald : Gem
    {
        public Emerald(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaEntityProps.SHADOW_SCALE, Vector3.one * 0.5f);
        }
    }
}