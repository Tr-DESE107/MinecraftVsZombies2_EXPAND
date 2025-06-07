﻿using PVZEngine;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    [PropertyRegistryRegion(PropertyRegions.placement)]
    public static class VanillaPlacementProps
    {
        public static readonly PropertyMeta<NamespaceID> ALMANAC_TAG = new PropertyMeta<NamespaceID>("almanacTag");
        public static NamespaceID GetAlmanacTag(this PlacementDefinition definition) => definition.GetProperty<NamespaceID>(ALMANAC_TAG);
        public static void SetAlmanacTag(this PlacementDefinition definition, NamespaceID value) => definition.SetProperty(ALMANAC_TAG, value);
    }
}
