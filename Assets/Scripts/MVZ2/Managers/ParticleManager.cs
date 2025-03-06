using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Models;
using UnityEngine;

namespace MVZ2.Managers
{
    public class ParticleManager : MonoBehaviour
    {
        public LightController PopLight()
        {
            LightController light;
            if (lightPool.Count <= 0)
            {
                if (lights.Count > maxCount)
                {
                    return null;
                }
                light = Instantiate(template.gameObject, lightRoot).GetComponent<LightController>();
                lights.Add(light);
            }
            else
            {
                light = lightPool.Pop();
            }
            light.gameObject.SetActive(true);
            return light;
        }
        public void PushLight(LightController light)
        {
            lightPool.Push(light);
            light.gameObject.SetActive(false);
        }

        private List<LightController> lights = new List<LightController>();
        private Stack<LightController> lightPool = new Stack<LightController>();
        [SerializeField]
        private Transform lightRoot;
        [SerializeField]
        private LightController template;
        [SerializeField]
        private int maxCount = 32;
    }
}
