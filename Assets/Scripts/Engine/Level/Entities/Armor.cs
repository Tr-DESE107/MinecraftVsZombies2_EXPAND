using System.Collections.Generic;
using PVZEngine.Definitions;
using PVZEngine.Serialization;
using Tools;
using UnityEngine;

namespace PVZEngine.Level
{
    public class Armor : IBuffTarget
    {
        private Armor()
        {

        }
        public Armor(Entity owner, ArmorDefinition definition)
        {
            Owner = owner;
            Definition = definition;
            Health = this.GetMaxHealth();

            SetProperty(EngineArmorProps.TINT, Color.white);
        }
        public void Update()
        {
            if (Definition != null)
                Definition.PostUpdate(this);
            foreach (var buff in buffs.GetAllBuffs())
            {
                buff.Update();
            }
        }
        public void Destroy(DamageResult result)
        {
            Owner.DestroyArmor(this, result);
        }

        #region 属性
        public object GetProperty(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            object result = null;
            if (propertyDict.TryGetProperty(name, out var prop))
                result = prop;
            else if (!ignoreDefinition && Definition != null)
                result = Definition.GetProperty<object>(name);

            if (!ignoreBuffs)
            {
                result = buffs.CalculateProperty(name, result);
            }
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            return GetProperty(name, ignoreDefinition, ignoreBuffs).ToGeneric<T>();
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
        }
        #endregion

        #region 增益
        public bool AddBuff(Buff buff)
        {
            if (buffs.AddBuff(buff))
            {
                buff.AddToTarget(this);
                return true;
            }
            return false;
        }
        public void AddBuff<T>() where T : BuffDefinition
        {
            AddBuff(Level.CreateBuff<T>());
        }
        public bool RemoveBuff(Buff buff) => buffs.RemoveBuff(buff);
        public int RemoveBuffs(IEnumerable<Buff> buffs) => this.buffs.RemoveBuffs(buffs);
        public bool HasBuff<T>() where T : BuffDefinition => buffs.HasBuff<T>();
        public bool HasBuff(Buff buff) => buffs.HasBuff(buff);
        public Buff[] GetBuffs<T>() where T : BuffDefinition => buffs.GetBuffs<T>();
        public Buff[] GetAllBuffs() => buffs.GetAllBuffs();
        #endregion
        public DamageResult TakeDamage(float amount, DamageEffectList effects, EntityReferenceChain source)
        {
            return TakeDamage(new DamageInfo(amount, effects, Owner, source));
        }
        public static DamageResult TakeDamage(DamageInfo info)
        {
            var entity = info.Entity;
            var armor = entity.EquipedArmor;
            if (!Exists(armor))
                return null;
            var shellRef = armor.GetProperty<NamespaceID>(EngineArmorProps.SHELL);
            var shell = entity.Level.ContentProvider.GetShellDefinition(shellRef);
            if (shell != null)
            {
                shell.EvaluateDamage(info);
            }
            // Apply Damage
            float hpBefore = armor.Health;
            if (info.Amount > 0)
            {
                armor.Health -= info.Amount;
            }
            bool fatal = hpBefore > 0 && armor.Health <= 0;
            var damageResult = new DamageResult()
            {
                OriginalDamage = info.OriginalDamage,
                Amount = info.Amount,
                UsedDamage = info.GetUsedDamage(),
                Entity = entity,
                Source = info.Source,
                Effects = info.Effects,
                Armor = armor,
                ShellDefinition = shell,
                Fatal = fatal
            };
            if (fatal)
            {
                armor.Destroy(damageResult);
            }
            return damageResult;
        }
        public static bool Exists(Armor armor)
        {
            return armor != null && armor.Owner != null && armor.Definition != null && armor.Health > 0;
        }
        public SerializableArmor Serialize()
        {
            return new SerializableArmor()
            {
                health = Health,
                definitionID = Definition.GetID(),
                buffs = buffs.ToSerializable(),
                propertyDict = propertyDict.Serialize()
            };
        }
        public static Armor Deserialize(SerializableArmor seri, Entity owner)
        {
            var definition = owner.Level.ContentProvider.GetArmorDefinition(seri.definitionID);
            var armor = new Armor();
            armor.Owner = owner;
            armor.Definition = definition;
            armor.Health = seri.health;
            armor.buffs = BuffList.FromSerializable(seri.buffs, owner.Level, armor);
            armor.propertyDict = PropertyDictionary.Deserialize(seri.propertyDict);
            return armor;
        }
        Entity IBuffTarget.GetEntity() => Owner;

        #region 属性字段
        public LevelEngine Level => Owner?.Level;
        public Entity Owner { get; set; }
        public ArmorDefinition Definition { get; private set; }
        public float Health { get; set; }
        private BuffList buffs = new BuffList();
        private PropertyDictionary propertyDict = new PropertyDictionary();
        #endregion
    }
}