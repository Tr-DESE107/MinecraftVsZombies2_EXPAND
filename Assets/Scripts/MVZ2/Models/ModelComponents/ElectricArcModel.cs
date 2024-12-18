using UnityEngine;

namespace MVZ2.Models
{
    public class ElectricArcModel : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            UpdateLines();
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var rng = Model.GetRNG();

            foreach (LightningGenerator lightning in lightnings)
            {
                LineRenderer renderer = lightning.LineRenderer;
                int count = renderer.positionCount;
                var parent = renderer.transform.parent;
                for (int i = 1; i < count - 1; i++)
                {
                    float percent = (float)i / count;
                    float modifier = -4 * Mathf.Pow(percent - 0.5f, 2) + 1;
                    Vector3 localPosition = renderer.GetPosition(i);
                    var worldPosition = parent.TransformPoint(localPosition);
                    worldPosition.x += rng.Next(-arcShiver, arcShiver) * modifier;
                    worldPosition.y += (rng.Next(-arcShiver, arcShiver) + arcLevitation) * modifier;
                    worldPosition.z += rng.Next(-arcShiver, arcShiver) * modifier;
                    localPosition = parent.InverseTransformPoint(worldPosition);
                    renderer.SetPosition(i, localPosition);
                }
            }
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var source = sourceTransform.position;
            var dest = Lawn2TransPosition(Model.GetProperty<Vector3>("Dest"));
            var distance = dest - source;
            sourceTransform.rotation = Quaternion.FromToRotation(Vector3.right, distance);
            sourceTransform.localScale = new Vector3(distance.magnitude, 1, 1);
        }
        private void UpdateLines()
        {
            foreach (LightningGenerator lightning in lightnings)
            {
                lightning.GenerateLightning(pointCount, sourceTransform.localPosition, destTransform.localPosition, Model.GetRNG());
            }
        }
        [SerializeField]
        private LightningGenerator[] lightnings;
        [SerializeField]
        private Transform sourceTransform;
        [SerializeField]
        private Transform destTransform;
        [SerializeField]
        private float arcShiver = 0.03f;
        [SerializeField]
        private float arcLevitation = 0.01f;
        [SerializeField]
        private int pointCount = 50;
    }
}
