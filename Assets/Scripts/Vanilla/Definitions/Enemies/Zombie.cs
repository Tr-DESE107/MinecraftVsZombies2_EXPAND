using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    public class Zombie : MeleeEnemy
    {
        public Zombie()
        {
            SetProperty(EntityProperties.SIZE, new Vector3(32, 86, 32));
        }
    }
}
