﻿using MVZ2.Level;
using MVZ2.Models;

namespace MVZ2.HeldItems
{
    public class HeldItemModelInterface : ModelInterface
    {
        public HeldItemModelInterface(LevelController level)
        {
            this.level = level;
        }
        protected override Model GetModel()
        {
            return level.GetHeldItemModel();
        }

        private LevelController level;
    }
}
