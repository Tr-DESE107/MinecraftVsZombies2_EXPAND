using System.Linq;
using PVZEngine.Armors;
using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class ArmorDamageResult : DamageResult
    {
        public Armor Armor { get; set; }
    }
}
