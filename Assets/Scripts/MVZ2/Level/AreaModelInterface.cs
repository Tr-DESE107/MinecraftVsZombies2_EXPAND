﻿using MVZ2.Level;
using MVZ2.Models;

namespace MVZ2.HeldItems
{
    public class AreaModelInterface : ModelInterface
    {
        public AreaModelInterface(LevelController level)
        {
            this.level = level;
        }
        protected override Model GetModel()
        {
            return level.GetAreaModel();
        }

        private LevelController level;
    }
}
