﻿namespace MVZ2.Models
{
    public class ModelParentInterface : ModelInterface
    {
        public ModelParentInterface(Model model)
        {
            this.model = model;
        }
        protected override Model GetModel()
        {
            return model;
        }
        protected Model model;
    }
}
