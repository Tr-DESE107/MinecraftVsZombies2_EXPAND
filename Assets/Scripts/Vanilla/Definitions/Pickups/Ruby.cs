using MVZ2.Vanilla;
using PVZEngine.Definitions;
using UnityEngine;

namespace MVZ2.GameContent
{
    [Definition(PickupNames.ruby)]
    public class Ruby : Gem
    {
        public Ruby(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.SIZE, new Vector3(32, 32, 32));
            SetProperty(PickupProps.MONEY_VALUE, 50);
        }
    }
}