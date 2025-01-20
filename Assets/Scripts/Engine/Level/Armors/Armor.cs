using System.Collections.Generic;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.Modifiers;
using UnityEngine;

namespace PVZEngine.Armors
{
    public class Armor : IBuffTarget, IPropertyModifyTarget
    {
        private Armor()
        {
            properties = new PropertyBlock(this);
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
            buffs.Update();
        }
        public void Destroy(ArmorDamageResult result)
        {
            Owner.DestroyArmor(this, result);
        }

        #region 属性
        public T GetProperty<T>(string name, bool ignoreBuffs = false)
        {
            return properties.GetProperty<T>(name, ignoreBuffs);
        }
        public void SetProperty(string name, object value)
        {
            properties.SetProperty(name, value);
        }
        private void UpdateAllBuffedProperties()
        {
            properties.UpdateAllModifiedProperties();
        }
        private void UpdateBuffedProperty(string name)
        {
            properties.UpdateModifiedProperty(name);
        }
        bool IPropertyModifyTarget.GetFallbackProperty(string name, out object value)
        {
            if (Definition != null)
            {
                if (Definition.TryGetProperty<object>(name, out var prop))
                {
                    value = prop;
                    return true;
                }
            }
            value = null;
            return false;
        }

        void IPropertyModifyTarget.GetModifierItems(string name, List<ModifierContainerItem> results)
        {
            buffs.GetModifierItems(name, results);
        }
        void IPropertyModifyTarget.UpdateModifiedProperty(string name, object value)
        {
        }
        PropertyModifier[] IPropertyModifyTarget.GetModifiersUsingProperty(string name)
        {
            return null;
        }
        IEnumerable<string> IPropertyModifyTarget.GetModifiedProperties()
        {
            return buffs.GetModifierPropertyNames();
        }
        #endregion

        #region 字段
        public T GetField<T>(string category, string name)
        {
            return properties.GetField<T>(category, name);
        }
        public void SetField(string category, string name, object value)
        {
            properties.SetField(category, name, value);
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
        public void GetBuffs<T>(List<Buff> results) where T : BuffDefinition => buffs.GetBuffsNonAlloc<T>(results);
        public void GetAllBuffs(List<Buff> results) => buffs.GetAllBuffs(results);
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
                properties = properties.ToSerializable()
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
            armor.properties = PropertyBlock.FromSerializable(seri.properties, armor);
            armor.UpdateAllBuffedProperties();
            return armor;
        }
        IModelInterface IBuffTarget.GetInsertedModel(NamespaceID key) => null;
        Entity IBuffTarget.GetEntity() => Owner;
        void IBuffTarget.GetBuffs(List<Buff> results) => buffs.GetAllBuffs(results);
        Buff IBuffTarget.GetBuff(long id) => buffs.GetBuff(id);
        bool IBuffTarget.Exists() => Owner != null && Owner.Exists() && Owner.EquipedArmor == this;

        #region 属性字段
        public LevelEngine Level => Owner?.Level;
        public Entity Owner { get; set; }
        public ArmorDefinition Definition { get; private set; }
        public float Health { get; set; }
        private long currentBuffID = 1;
        private BuffList buffs = new BuffList();
        private PropertyBlock properties;
        #endregion
    }
}