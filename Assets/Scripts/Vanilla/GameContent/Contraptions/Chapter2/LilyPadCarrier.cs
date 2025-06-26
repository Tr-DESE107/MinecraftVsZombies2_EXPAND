using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.lilyPadCarrier)]
    public class LilyPadCarrier : CarrierContraptionBehaviour
    {
        public LilyPadCarrier(string nsp, string name) : base(nsp, name)
        {
        }
        protected override NamespaceID GetCarrierBuffID() => VanillaBuffID.carryingOther;
        protected override NamespaceID GetPassenagerBuffID() => VanillaBuffID.carriedByLilyPad;
    }
}
