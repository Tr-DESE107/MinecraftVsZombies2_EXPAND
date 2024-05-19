using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaContraption : EntityDefinition, IEvokableContraption
    {
        public virtual void Evoke(Contraption contraption)
        {
        }
        public override int Type => EntityTypes.CONTRAPTION;

    }
    public interface IEvokableContraption
    {
        void Evoke(Contraption contraption);
    }
}