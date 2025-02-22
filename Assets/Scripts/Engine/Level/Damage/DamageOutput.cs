using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class DamageOutput
    {
        public Entity Entity { get; set; }
        public BodyDamageResult BodyResult { get; set; }
        public ArmorDamageResult ArmorResult { get; set; }
        public ArmorDamageResult ShieldResult { get; set; }
        public bool HasAnyNotFatal()
        {
            if (ArmorResult != null && ArmorResult.Fatal)
                return false;
            if (BodyResult != null && BodyResult.Fatal)
                return false;
            if (ShieldResult != null && ShieldResult.Fatal)
                return false;
            return true;
        }
        public bool HasAnyFatal()
        {
            if (ArmorResult != null && ArmorResult.Fatal)
                return true;
            if (BodyResult != null && BodyResult.Fatal)
                return true;
            if (ShieldResult != null && ShieldResult.Fatal)
                return true;
            return false;
        }
    }
}
