﻿using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public static class VanillaSpawnConditions
    {
        public static readonly SpawnCondition any = new AnySpawnCondition();
        public static readonly SpawnCondition normal = new NormalSpawnCondition();
        public static readonly SpawnCondition buried = new BuriedSpawnCondition();
        public static readonly SpawnCondition aquatic = new AquaticSpawnCondition();
        public static readonly SpawnCondition pad = new PadSpawnCondition();
        public static readonly SpawnCondition dreamSilk = new DreamSilkSpawnCondition();
        public static readonly SpawnCondition suspension = new SuspensionSpawnCondition();
        public static readonly SpawnCondition devourer = new DevourerSpawnCondition();
    }
}
