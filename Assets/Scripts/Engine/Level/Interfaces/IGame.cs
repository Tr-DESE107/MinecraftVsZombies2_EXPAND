using PVZEngine.Definitions;

namespace PVZEngine.Base
{
    public interface IGame
    {
        EntityDefinition GetEntityDefinition(NamespaceID defRef);
        T GetEntityDefinition<T>() where T : EntityDefinition;
        SeedDefinition GetSeedDefinition(NamespaceID seedRef);
        RechargeDefinition GetRechargeDefinition(NamespaceID seedRef);
        ShellDefinition GetShellDefinition(NamespaceID defRef);
        AreaDefinition GetAreaDefinition(NamespaceID defRef);
        StageDefinition GetStageDefinition(NamespaceID defRef);
        GridDefinition GetGridDefinition(NamespaceID defRef);
        BuffDefinition GetBuffDefinition(NamespaceID defRef);
        T GetBuffDefinition<T>() where T : BuffDefinition;
        ArmorDefinition GetArmorDefinition(NamespaceID defRef);
        T GetArmorDefinition<T>() where T : ArmorDefinition;
        SpawnDefinition GetSpawnDefinition(NamespaceID defRef);
        string GetText(string textKey);
        string GetTextParticular(string textKey, string context);
    }
}
