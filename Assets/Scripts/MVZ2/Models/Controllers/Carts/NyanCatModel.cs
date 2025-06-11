﻿using UnityEngine;

namespace MVZ2.Models
{
    public class NyanCatModel : ModelComponent
    {
        public override void OnPropertySet(string name, object value)
        {
            base.OnPropertySet(name, value);
            if (name == "Nyaightmare")
            {
                UpdateNyanCat();
            }
        }
        private void UpdateNyanCat()
        {
            var nyaightmare = Model.GetProperty<bool>("Nyaightmare");
            nyanCatRoot.SetActive(!nyaightmare);
            nyaightmareRoot.SetActive(nyaightmare);
        }
        [SerializeField]
        private GameObject nyanCatRoot;
        [SerializeField]
        private GameObject nyaightmareRoot;
    }
}
