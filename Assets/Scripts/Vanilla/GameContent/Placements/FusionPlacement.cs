using MVZ2.GameContent.Placements;
using PVZEngine.Definitions;
using PVZEngine.Placements;

[AutoPlacementDefinition(VanillaPlacementNames.fusion)]
public class FusionPlacement : PlacementDefinition
{
    public FusionPlacement(string nsp, string name) : base(nsp, name, VanillaSpawnConditions.normal)
    {
        AddMethod(VanillaPlaceMethods.fusion);
        AddMethod(VanillaPlaceMethods.entity);
    }
}