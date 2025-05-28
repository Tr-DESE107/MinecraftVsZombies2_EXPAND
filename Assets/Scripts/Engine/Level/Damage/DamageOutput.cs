﻿using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class DamageOutput
    {
        public Entity Entity { get; set; }
        public BodyDamageResult BodyResult { get; set; }
        public ArmorDamageResult ArmorResult { get; set; }
        public ArmorDamageResult ShieldResult { get; set; }
        public NamespaceID ShieldTarget { get; set; }
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
        public float GetTotalAmount()
        {
            float sum = 0;
            if (ArmorResult != null)
                sum += ArmorResult.Amount;
            if (BodyResult != null)
                sum += BodyResult.Amount;
            if (ShieldResult != null)
                sum += ShieldResult.Amount;
            return sum;
        }
        public float GetTotalSpendAmount()
        {
            float sum = 0;
            if (ArmorResult != null)
                sum += ArmorResult.SpendAmount;
            if (BodyResult != null)
                sum += BodyResult.SpendAmount;
            if (ShieldResult != null)
                sum += ShieldResult.SpendAmount;
            return sum;
        }
        public DamageResult[] GetAllResults()
        {
            return new DamageResult[]
            {
                BodyResult,
                ArmorResult,
                ShieldResult
            };
        }
    }
}
