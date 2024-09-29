using MVZ2.Vanilla;
using UnityEngine;

namespace MVZ2.GameContent
{
    [Definition(PickupNames.emerald)]
    public class Emerald : Gem
    {
        public Emerald(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinEntityProps.SHADOW_SCALE, Vector3.one * 0.5f);
        }
    }
}