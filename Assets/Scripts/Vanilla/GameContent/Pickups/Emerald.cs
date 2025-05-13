using System.Collections.Generic;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.Rendering;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.emerald)]
    public class Emerald : Gem
    {
        public Emerald(string nsp, string name) : base(nsp, name)
        {
        }
        protected override bool CanMerge => true;
        protected override int MergeCount => 5;
        protected override NamespaceID MergeSource => VanillaPickupID.emerald;
        protected override NamespaceID MergeTarget => VanillaPickupID.ruby;

    }
}