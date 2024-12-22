using System.Collections.Generic;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace PVZEngine.Armors
{
    public class Armor : IBuffTarget
    {
        private Armor()
        {
            buffs.OnPropertyChanged += UpdateBuffedProperty;
        }
        public Armor(Entity owner, ArmorDefinition definition) : this()
        {
            Owner = owner;
            Definition = definition;
            Health = this.GetMaxHealth();

            SetProperty(EngineArmorProps.TINT, Color.white);
        }
        public void Update()
        {
            Health = Mathf.Min(Health, this.GetMaxHealth());
            if (Definition != null)
                Definition.PostUpdate(this);
            foreach (var buff in buffs.GetAllBuffs())
            {
                buff.Update();
            }
        }
        public void Destroy(ArmorDamageResult result)
        {
            Owner.DestroyArmor(this, result);
        }

        #region 属性
        public object GetProperty(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            if (!ignoreBuffs)
            {
                if (buffedProperties.TryGetProperty(name, out var value))
                    return value;
            }
            object result = null;
            if (propertyDict.TryGetProperty(name, out var prop))
                result = prop;
            else if (!ignoreDefinition && Definition != null)
                result = Definition.GetProperty<object>(name);
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            return GetProperty(name, ignoreDefinition, ignoreBuffs).ToGeneric<T>();
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
            UpdateBuffedProperty(name);
        }
        private void UpdateAllBuffedProperties()
        {
            var propertyNames = buffs.GetModifierPropertyNames();
            foreach (var name in propertyNames)
            {
                UpdateBuffedProperty(name);
            }
        }
        private void UpdateBuffedProperty(string name)
        {
            var baseValue = GetProperty(name, ignoreBuffs: true);
            var value = buffs.CalculateProperty(name, baseValue);
            buffedProperties.SetProperty(name, value);
        }
        #endregion

        #region 增益
        public Buff CreateBuff<T>() where T : BuffDefinition
        {
            return Level.CreateBuff<T>(AllocBuffID());
        }
        public Buff CreateBuff(BuffDefinition buffDef)
        {
            return Level.CreateBuff(buffDef, AllocBuffID());
        }
        public Buff CreateBuff(NamespaceID id)
        {
            return Level.CreateBuff(id, AllocBuffID());
        }
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
            AddBuff(CreateBuff<T>());
        }
        public bool RemoveBuff(Buff buff) => buffs.RemoveBuff(buff);
        public int RemoveBuffs(IEnumerable<Buff> buffs) => this.buffs.RemoveBuffs(buffs);
        public bool HasBuff<T>() where T : BuffDefinition => buffs.HasBuff<T>();
        public bool HasBuff(Buff buff) => buffs.HasBuff(buff);
        public Buff[] GetBuffs<T>() where T : BuffDefinition => buffs.GetBuffs<T>();
        public Buff[] GetAllBuffs() => buffs.GetAllBuffs();
        public BuffReference GetBuffReference(Buff buff) => new BuffReferenceArmor(Owner.ID, buff.ID);
        private long AllocBuffID()
        {
            return currentBuffID++;
        }
        #endregion
        public ArmorDamageResult TakeDamage(float amount, DamageEffectList effects, EntityReferenceChain source)
        {
            return TakeDamage(new DamageInput(amount, effects, Owner, source));
        }
        public static ArmorDamageResult TakeDamage(DamageInput info)
        {
            var entity = info.Entity;
            var armor = entity.EquipedArmor;
            if (!Exists(armor))
                return null;
            var shellRef = armor.GetProperty<NamespaceID>(EngineArmorProps.SHELL);
            var shell = entity.Level.Content.GetShellDefinition(shellRef);
            if (shell != null)
            {
                shell.EvaluateDamage(info);
            }
            // Apply Damage
            float hpBefore = armor.Health;
            var amount = info.Amount;
            if (amount > 0)
            {
                armor.Health -= amount;
            }
            bool fatal = hpBefore > 0 && armor.Health <= 0;
            var damageResult = new ArmorDamageResult()
            {
                OriginalAmount = info.OriginalAmount,
                Amount = amount,
                Armor = armor,
                Effects = info.Effects,
                Entity = entity,
                Source = info.Source,
                SpendAmount = Mathf.Min(hpBefore, amount),
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
                currentBuffID = currentBuffID,
                definitionID = Definition.GetID(),
                buffs = buffs.ToSerializable(),
                propertyDict = propertyDict.Serialize()
            };
        }
        public static Armor Deserialize(SerializableArmor seri, Entity owner)
        {
            var definition = owner.Level.Content.GetArmorDefinition(seri.definitionID);
            var armor = new Armor();
            armor.Owner = owner;
            armor.Definition = definition;
            armor.Health = seri.health;
            armor.currentBuffID = seri.currentBuffID;
            armor.buffs = BuffList.FromSerializable(seri.buffs, owner.Level, armor);
            armor.buffs.OnPropertyChanged += armor.UpdateBuffedProperty;
            armor.propertyDict = PropertyDictionary.Deserialize(seri.propertyDict);
            armor.UpdateAllBuffedProperties();
            return armor;
        }
        Entity IBuffTarget.GetEntity() => Owner;
        IEnumerable<Buff> IBuffTarget.GetBuffs() => buffs.GetAllBuffs();
        bool IBuffTarget.Exists() => Owner != null && Owner.Exists() && Owner.EquipedArmor == this;

        #region 属性字段
        public LevelEngine Level => Owner?.Level;
        public Entity Owner { get; set; }
        public ArmorDefinition Definition { get; private set; }
        public float Health { get; set; }
        private long currentBuffID = 1;
        private BuffList buffs = new BuffList();
        private PropertyDictionary buffedProperties = new PropertyDictionary();
        private PropertyDictionary propertyDict = new PropertyDictionary();
        #endregion
    }
}