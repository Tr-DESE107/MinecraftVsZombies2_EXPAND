using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PVZEngine
{
    public class Armor : IBuffTarget
    {
        public Armor(Entity owner, ArmorDefinition definition)
        {
            Owner = owner;
            Definition = definition;
            Health = GetMaxHealth();

            SetProperty(ArmorProperties.TINT, Color.white);
        }
        public void Update()
        {
            if (Definition != null)
                Definition.PostUpdate(this);
        }
        public void Destroy(DamageResult result)
        {
            Owner.DestroyArmor(this, result);
        }

        #region 属性
        public object GetProperty(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            object result = null;
            if (propertyDict.TryGetValue(name, out var prop))
                result = prop;
            else if (!ignoreDefinition && Definition != null)
                result = Definition.GetProperty<object>(name);

            if (!ignoreBuffs)
            {
                foreach (var buff in buffs)
                {
                    foreach (var modi in buff.GetModifiers())
                    {
                        if (modi.PropertyName != name)
                            continue;
                        result = modi.CalculateProperty(buff, result);
                    }
                }
            }
            return result;
        }
        public T GetProperty<T>(string name, bool ignoreDefinition = false, bool ignoreBuffs = false)
        {
            return PropertyDictionary.ToGeneric<T>(GetProperty(name, ignoreDefinition, ignoreBuffs));
        }
        public void SetProperty(string name, object value)
        {
            propertyDict[name] = value;
        }
        #endregion

        #region 原版属性
        public NamespaceID GetShellID(bool ignoreBuffs = false)
        {
            return GetProperty<NamespaceID>(EntityProperties.SHELL, ignoreBuffs: ignoreBuffs);
        }
        public void SetShellID(NamespaceID value)
        {
            SetProperty(EntityProperties.SHELL, value);
        }
        public Color GetTint(bool ignoreBuffs = false)
        {
            return GetProperty<Color>(ArmorProperties.TINT, ignoreBuffs: ignoreBuffs);
        }
        public void SetTint(Color value)
        {
            SetProperty(ArmorProperties.TINT, value);
        }
        public Color GetColorOffset(bool ignoreBuffs = false)
        {
            return GetProperty<Color>(ArmorProperties.COLOR_OFFSET, ignoreBuffs: ignoreBuffs);
        }
        public void SetColorOffset(Color value)
        {
            SetProperty(ArmorProperties.COLOR_OFFSET, value);
        }
        public float GetMaxHealth(bool ignoreBuffs = false)
        {
            return GetProperty<float>(EntityProperties.MAX_HEALTH, ignoreBuffs: ignoreBuffs);
        }
        public void SetMaxHealth(float value)
        {
            SetProperty(EntityProperties.MAX_HEALTH, value);
        }
        #endregion

        #region 增益
        public void AddBuff(Buff buff)
        {
            if (buff == null)
                return;
            buffs.Add(buff);
            buff.AddToTarget(this);
        }
        public void AddBuff<T>() where T : BuffDefinition
        {
            AddBuff(Owner.Game.CreateBuff<T>());
        }
        public bool RemoveBuff(Buff buff)
        {
            if (buff == null)
                return false;
            if (buffs.Remove(buff))
            {
                buff.RemoveFromTarget();
                return true;
            }
            return false;
        }
        public int RemoveBuffs(IEnumerable<Buff> buffs)
        {
            if (buffs == null)
                return 0;
            int count = 0;
            foreach (var buff in buffs)
            {
                count += RemoveBuff(buff) ? 1 : 0;
            }
            return count;
        }
        public bool HasBuff<T>() where T : BuffDefinition
        {
            return buffs.Any(b => b.Definition is T);
        }
        public bool HasBuff(Buff buff)
        {
            return buffs.Contains(buff);
        }
        public Buff[] GetBuffs<T>() where T : BuffDefinition
        {
            return buffs.Where(b => b.Definition is T).ToArray();
        }
        public Buff[] GetAllBuffs()
        {
            return buffs.ToArray();
        }
        #endregion
        public DamageResult TakeDamage(float amount, DamageEffectList effects, EntityReference source)
        {
            return TakeDamage(new DamageInfo(amount, effects, Owner, source));
        }
        public static DamageResult TakeDamage(DamageInfo info)
        {
            var entity = info.Entity;
            var armor = entity.EquipedArmor;
            if (!Exists(armor))
                return null;
            var shellRef = armor.GetProperty<NamespaceID>(ArmorProperties.SHELL);
            var shell = entity.Game.GetShellDefinition(shellRef);
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

        #region 属性字段
        public Entity Owner { get; set; }
        public ArmorDefinition Definition { get; private set; }
        public float Health { get; set; }
        private List<Buff> buffs = new List<Buff>();
        private Dictionary<string, object> propertyDict = new Dictionary<string, object>();
        #endregion
    }
}