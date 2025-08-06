using System.Collections.Generic;
using System.Linq;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Level.Collisions;
using PVZEngine.Models;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace PVZEngine.Armors
{
    public class Armor : IBuffTarget, IPropertyModifyTarget, IAuraSource
    {
        private Armor()
        {
            properties = new PropertyBlock(this);
            buffs.OnPropertyChanged += UpdateBuffedProperty;
        }
        public Armor(Entity owner, NamespaceID slot, ArmorDefinition definition) : this()
        {
            Owner = owner;
            Slot = slot;
            Definition = definition;
            Health = this.GetMaxHealth();

            CreateAuraEffects();
        }
        public void Update()
        {
            Health = Mathf.Min(Health, this.GetMaxHealth());
            if (Definition != null)
                Definition.PostUpdate(this);
            auras.Update();
            buffs.Update();
        }
        public void Destroy(ArmorDestroyInfo result = null)
        {
            result = result ?? new ArmorDestroyInfo(Owner, this, Slot, new DamageEffectList(), new EntityReferenceChain(null), null);
            Owner.DestroyArmor(Slot, result);
        }
        public void PostAdd()
        {
            auras.PostAdd();
        }
        public void PostRemove()
        {
            auras.PostRemove();
        }
        public IEnumerable<ColliderConstructor> GetColliderConstructors(Entity entity, NamespaceID slot)
        {
            return Definition.GetColliderConstructors(entity, slot);
        }

        #region 动画
        public IModelInterface GetModelInterface()
        {
            var key = EngineArmorExt.GetModelKeyOfArmorSlot(Slot);
            return Owner.GetChildModel(key);
        }
        public void TriggerAnimation(string name)
        {
            GetModelInterface()?.TriggerAnimation(name);
        }
        public void SetAnimationBool(string name, bool value)
        {
            GetModelInterface()?.SetAnimationBool(name, value);
        }
        public void SetAnimationInt(string name, int value)
        {
            GetModelInterface()?.SetAnimationInt(name, value);
        }
        public void SetAnimationFloat(string name, float value)
        {
            GetModelInterface()?.SetAnimationFloat(name, value);
        }
        public void SetModelProperty(string name, object value)
        {
            GetModelInterface()?.SetModelProperty(name, value);
        }
        #endregion

        #region 属性
        public T GetProperty<T>(PropertyKey<T> name, bool ignoreBuffs = false)
        {
            return properties.GetProperty<T>(name, ignoreBuffs);
        }
        public void SetProperty<T>(PropertyKey<T> name, T value)
        {
            properties.SetProperty(name, value);
        }
        private void UpdateAllBuffedProperties(bool triggersEvaluation)
        {
            properties.UpdateAllModifiedProperties(triggersEvaluation);
        }
        private void UpdateBuffedProperty(IPropertyKey name)
        {
            properties.UpdateModifiedProperty(name);
        }
        bool IPropertyModifyTarget.GetFallbackProperty(IPropertyKey name, out object value)
        {
            if (Definition != null)
            {
                if (Definition.TryGetPropertyObject(name, out var prop))
                {
                    value = prop;
                    return true;
                }
            }
            value = default;
            return false;
        }

        void IPropertyModifyTarget.GetModifierItems(IPropertyKey name, List<ModifierContainerItem> results)
        {
            buffs.GetModifierItems(name, results);
        }
        void IPropertyModifyTarget.UpdateModifiedProperty(IPropertyKey name, object beforeValue, object afterValue, bool triggersEvaluation)
        {
            if (triggersEvaluation)
            {
                if (EngineArmorProps.MAX_HEALTH.Equals(name))
                {
                    var before = beforeValue.ToGeneric<float>();
                    var after = afterValue.ToGeneric<float>();
                    Health = Mathf.Min(after, Health * (after / before));
                }
            }
        }
        PropertyModifier[] IPropertyModifyTarget.GetModifiersUsingProperty(IPropertyKey name)
        {
            return null;
        }
        IEnumerable<IPropertyKey> IPropertyModifyTarget.GetModifiedProperties()
        {
            return buffs.GetModifierPropertyNames();
        }
        #endregion

        #region 增益
        public Buff NewBuff<T>() where T : BuffDefinition
        {
            return Level.CreateBuff<T>(AllocBuffID());
        }
        public Buff NewBuff(BuffDefinition buffDef)
        {
            return Level.CreateBuff(buffDef, AllocBuffID());
        }
        public Buff NewBuff(NamespaceID id)
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
            AddBuff(NewBuff<T>());
        }
        public bool RemoveBuff(Buff buff) => buffs.RemoveBuff(buff);
        public int RemoveBuffs(IEnumerable<Buff> buffs) => this.buffs.RemoveBuffs(buffs);
        public bool HasBuff<T>() where T : BuffDefinition => buffs.HasBuff<T>();
        public bool HasBuff(Buff buff) => buffs.HasBuff(buff);
        public void GetBuffs<T>(List<Buff> results) where T : BuffDefinition => buffs.GetBuffsNonAlloc<T>(results);
        public void GetAllBuffs(List<Buff> results) => buffs.GetAllBuffs(results);
        public BuffReference GetBuffReference(Buff buff) => new BuffReferenceArmor(Owner.ID, Slot, buff.ID);
        private long AllocBuffID()
        {
            return currentBuffID++;
        }
        #endregion

        #region 光环
        public AuraEffect GetAuraEffect<T>() where T : AuraEffectDefinition
        {
            return auras.Get<T>();
        }
        public AuraEffect[] GetAuraEffects()
        {
            return auras.GetAll();
        }
        private void CreateAuraEffects()
        {
            var auraDefs = Definition.GetAuras();
            for (int i = 0; i < auraDefs.Length; i++)
            {
                var auraDef = auraDefs[i];
                auras.Add(Level, new AuraEffect(auraDef, i, this));
            }
        }
        #endregion

        public static bool Exists(Armor armor)
        {
            return armor != null && armor.Owner != null && armor.Definition != null && armor.Health > 0;
        }
        public SerializableArmor Serialize()
        {
            return new SerializableArmor()
            {
                health = Health,
                slot = Slot,
                currentBuffID = currentBuffID,
                definitionID = Definition.GetID(),
                buffs = buffs.ToSerializable(),
                properties = properties.ToSerializable(),
                auras = auras.GetAll().Select(a => a.ToSerializable()).ToArray()
            };
        }
        public static Armor Deserialize(SerializableArmor seri, Entity owner)
        {
            var definition = owner.Level.Content.GetArmorDefinition(seri.definitionID);
            var armor = new Armor();
            armor.Owner = owner;
            armor.Definition = definition;
            armor.Slot = seri.slot;
            armor.Health = seri.health;
            armor.currentBuffID = seri.currentBuffID;
            armor.buffs = BuffList.FromSerializable(seri.buffs, owner.Level, armor);
            armor.buffs.OnPropertyChanged += armor.UpdateBuffedProperty;
            armor.properties = PropertyBlock.FromSerializable(seri.properties, armor);
            armor.UpdateAllBuffedProperties(false);
            return armor;
        }
        public void LoadAuras(SerializableArmor seri)
        {
            buffs.LoadAuras(seri.buffs, Level);

            CreateAuraEffects();
            auras.LoadFromSerializable(Level, seri.auras);
        }
        IModelInterface IBuffTarget.GetInsertedModel(NamespaceID key) => null;
        Entity IBuffTarget.GetEntity() => Owner;
        Armor IBuffTarget.GetArmor() => this;
        void IBuffTarget.GetBuffs(List<Buff> results) => buffs.GetAllBuffs(results);
        Buff IBuffTarget.GetBuff(long id) => buffs.GetBuff(id);
        bool IBuffTarget.Exists() => Owner != null && Owner.Exists() && Owner.IsEquippingArmor(this);
        Entity IAuraSource.GetEntity() => Owner;
        LevelEngine IAuraSource.GetLevel() => Level;
        bool IAuraSource.IsValid() => Owner != null && Owner.Exists() && Owner.IsEquippingArmor(this);

        #region 属性字段
        public LevelEngine Level => Owner?.Level;
        public Entity Owner { get; set; }
        public NamespaceID Slot { get; set; }
        public ArmorDefinition Definition { get; private set; }
        public float Health { get; set; }
        private long currentBuffID = 1;
        private BuffList buffs = new BuffList();
        private PropertyBlock properties;
        private AuraEffectList auras = new AuraEffectList();
        #endregion
    }
}