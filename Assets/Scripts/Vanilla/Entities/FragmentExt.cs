using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public static class FragmentExt
    {
        public static void InitFragment(this Entity entity)
        {
            var fragment = entity.CreateFragment();
            var fragmentRef = new EntityID(fragment);
            entity.SetFragment(fragmentRef);
        }
        public static void UpdateFragment(this Entity entity)
        {
            var fragment = entity.GetOrCreateFragment();
            Fragment.AddEmitSpeed(fragment, entity.GetFragmentTickDamage() * 0.1f);
            entity.SetFragmentTickDamage(0);
        }
        public static void PostFragmentDeath(this Entity entity, DamageInfo damageInfo)
        {
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            var fragment = entity.GetOrCreateFragment();
            Fragment.AddEmitSpeed(fragment, 50);
        }
        public static EntityID GetFragment(this Entity entity)
        {
            return entity.GetProperty<EntityID>("Fragment");
        }
        public static void SetFragment(this Entity entity, EntityID value)
        {
            entity.SetProperty("Fragment", value);
        }
        public static float GetFragmentTickDamage(this Entity entity)
        {
            return entity.GetProperty<float>("TickDamage");
        }
        public static void SetFragmentTickDamage(this Entity entity, float value)
        {
            entity.SetProperty("TickDamage", value);
        }
        public static void AddFragmentTickDamage(this Entity entity, float value)
        {
            entity.SetFragmentTickDamage(entity.GetFragmentTickDamage() + value);
        }
        public static Entity CreateFragment(this Entity entity)
        {
            var fragment = entity.Level.Spawn<Fragment>(entity.Position, entity);
            fragment.SetParent(entity);
            return fragment;
        }
        public static Entity GetOrCreateFragment(this Entity entity)
        {
            var fragmentRef = entity.GetFragment();
            var fragment = fragmentRef?.GetEntity(entity.Level);
            if (fragment == null || !fragment.Exists())
            {
                fragment = entity.CreateFragment();
                fragmentRef = new EntityID(fragment);
                entity.SetFragment(fragmentRef);
            }
            return fragment;
        }
    }
}
