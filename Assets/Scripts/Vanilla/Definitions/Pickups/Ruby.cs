using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent
{
    [Definition(VanillaPickupNames.ruby)]
    public class Ruby : Gem
    {
        public Ruby(string nsp, string name) : base(nsp, name)
        {
        }
    }
}