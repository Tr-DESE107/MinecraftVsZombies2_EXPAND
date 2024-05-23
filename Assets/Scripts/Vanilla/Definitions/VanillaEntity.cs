using MVZ2.GameContent;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaEntity : EntityDefinition
    {
        protected VanillaEntity() : base()
        {
            SetProperty(EntityProps.PLACE_SOUND, SoundID.grass);
        }
    }
}
