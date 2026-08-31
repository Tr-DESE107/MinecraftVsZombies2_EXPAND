#nullable enable

using MVZ2.GameContent.Effects;
using MVZ2Logic.Contents.Enemies;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Seeds  
{  
    public static class HeavyWeaponBlueprintUtils  
    {  
        // 找到当前坐在矿车上的器械（通用：不限于超级狙击发射器）  
        public static Entity? FindRider(LevelEngine level)  
        {  
            var cart = level.FindFirstEntity(VanillaEffectID.minecartRideable);  
            if (!cart.ExistsAndAlive())  
                return null;  
            return cart!.GetRideablePassenger();  
        }  
        public static Entity? FindCart(LevelEngine level)  
        {  
            var cart = level.FindFirstEntity(VanillaEffectID.minecartRideable);  
            return cart.ExistsAndAlive() ? cart : null;  
        }  
    }  
}
