using System.Collections.Generic;
using System.Linq;
using PVZEngine.Level;

namespace PVZEngine.Auras
{
    public class AuraEffectList
    {
        public AuraEffect CreateAura(AuraEffectDefinition auraDef, int id)
        {
            return new AuraEffect(auraDef, id);
        }
        public void Add(LevelEngine level, AuraEffect auraEffect)
        {
            auraEffects.Add(auraEffect);
            auraEffect.PostAdd(level);
        }
        public bool Remove(LevelEngine level, AuraEffect auraEffect)
        {
            if (auraEffects.Remove(auraEffect))
            {
                auraEffect.PostRemove(level);
                return true;
            }
            return true;
        }
        public void PostAdd(LevelEngine level)
        {
            foreach (var auraEffect in auraEffects)
            {
                auraEffect.PostAdd(level);
            }
        }
        public void PostRemove(LevelEngine level)
        {
            foreach (var auraEffect in auraEffects)
            {
                auraEffect.PostRemove(level);
            }
        }
        public AuraEffect Get(AuraEffectDefinition auraDef)
        {
            return auraEffects.FirstOrDefault(a => a.Definition == auraDef);
        }
        public AuraEffect[] GetAll()
        {
            return auraEffects.ToArray();
        }
        public void Clear(LevelEngine level)
        {
            foreach (var auraEffect in auraEffects)
            {
                auraEffect.PostRemove(level);
            }
            auraEffects.Clear();
        }
        public void Update(LevelEngine level)
        {
            foreach (var aura in auraEffects)
            {
                aura.UpdateAuraInterval(level);
            }
        }
        public void LoadFromSerializable(LevelEngine level, IEnumerable<SerializableAuraEffect> effects)
        {
            foreach (var aura in auraEffects)
            {
                var seri = effects.FirstOrDefault(e => e.id == aura.ID);
                aura.LoadFromSerializable(level, seri);
            }
        }
        private List<AuraEffect> auraEffects = new List<AuraEffect>();
    }
}
