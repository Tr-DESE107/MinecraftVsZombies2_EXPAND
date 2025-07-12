using PVZEngine;

namespace MVZ2Logic.SeedPacks
{
    public interface IEntitySeedMeta
    {
        string ID { get; }
        int GetCost();
        SpriteReference GetIcon();
        NamespaceID GetModelID();
        NamespaceID GetEntityID();
        NamespaceID GetRechargeID();
        bool IsTriggerActive();
        bool CanInstantTrigger();
        bool IsUpgradeBlueprint();
        bool CanInstantEvoke();
        int GetVariant();
    }
}
