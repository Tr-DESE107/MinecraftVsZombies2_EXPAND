using MVZ2.Managers;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Rendering
{
    public class FragmentColorSetter : MonoBehaviour
    {
        private void Update()
        {
            var fragID = model.GetProperty<NamespaceID>("FragmentID");
            if (lastID != fragID)
            {
                lastID = fragID;
                var main = particles.main;
                var gradient = defaultGradient;
                if (fragID != null)
                {
                    var resourceManager = MainManager.Instance.ResourceManager;
                    gradient = resourceManager.GetFragmentGradient(fragID) ?? defaultGradient;
                }
                main.startColor = new ParticleSystem.MinMaxGradient()
                {
                    mode = ParticleSystemGradientMode.RandomColor,
                    gradient = gradient,
                };
            }
        }
        public static readonly Gradient defaultGradient = new Gradient()
        {
            mode = GradientMode.Fixed,
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.magenta, 0.5f),
                new GradientColorKey(Color.black, 1)
            }
        };
        [SerializeField]
        private Model model;
        [SerializeField]
        private ParticleSystem particles;
        private NamespaceID lastID;
    }
}
