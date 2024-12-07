using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class DamageOutput
    {
        public Entity Entity { get; set; }
        public BodyDamageResult BodyResult { get; set; }
        public ArmorDamageResult ArmorResult { get; set; }
        public ArmorDamageResult ShieldResult { get; set; }
    }
}
