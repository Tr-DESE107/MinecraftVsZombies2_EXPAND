using System.Collections.Generic;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Projectiles;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public class Vanilla : Mod
    {
        public Vanilla() : base("mvz2")
        {
            AddDefinition(areaDefinitions,"day", new Day());
            AddDefinition(stageDefinitions, "prologue", new StageDefinition());

            AddDefinition(entityDefinitions, "dispenser", new Dispenser());
            AddDefinition(entityDefinitions, "zombie", new Zombie());
            AddDefinition(entityDefinitions, "arrow", new Arrow());
        }

        protected void AddDefinition<T>(Dictionary<string, T> list, string name, T definition) 
            where T: Definition
        {
            definition.Namespace = Namespace;
            definition.Name = name;
            list.Add(name, definition);
        }
    }
}
