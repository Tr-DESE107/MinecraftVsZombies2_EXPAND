﻿using MVZ2.Level;
using MVZ2.Models;

namespace MVZ2.SeedPacks
{
    public class BlueprintModelInterface : ModelInterface
    {
        public BlueprintModelInterface(BlueprintController ctrl)
        {
            this.controller = ctrl;
        }
        protected override Model GetModel()
        {
            return controller.GetModel();
        }
        protected BlueprintController controller;
    }
}
