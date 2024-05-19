using System.Collections.Generic;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Contraptions;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public class Vanilla : Mod
    {
        public Vanilla() : base("mvz2")
        {
            var dayDefinition = new Day()
            {
                Namespace = Namespace,
                Name = "day"
            };
            areaDefinitions.Add("day", dayDefinition);

            var stageDefinition = new StageDefinition()
            {
                Namespace = Namespace,
                Name = "prologue"
            };
            stageDefinitions.Add("prologue", stageDefinition);

            var dispenser = new Dispenser()
            {
                Namespace = Namespace,
                Name = "dispenser"
            };
            entityDefinitions.Add("dispenser", dispenser);
        }
    }
}
