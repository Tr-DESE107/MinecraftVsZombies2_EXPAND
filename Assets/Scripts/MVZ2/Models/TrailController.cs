using UnityEngine;

namespace MVZ2.Models
{
    public class TrailController : MonoBehaviour
    {
        public void Init()
        {
            trailPoints = new Vector3[trail.positionCount];
            trail.GetPositions(trailPoints);
            for (int i = 0; i < trailPoints.Length; i++)
            {
                trailPoints[i] = transform.position;
            }
            trail.SetPositions(trailPoints);
        }
        public void UpdateLogic()
        {
            trail.GetPositions(trailPoints);
            for (int i = trailPoints.Length - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    trailPoints[i] = transform.position;
                }
                else
                {
                    trailPoints[i] = trailPoints[i - 1] + velocity * i;
                }
            }
            trail.SetPositions(trailPoints);
        }
        public void UpdateFrame()
        {
            trail.SetPosition(0, transform.position);
        }
        [SerializeField]
        private LineRenderer trail;
        [SerializeField]
        private Vector3 velocity;
        private Vector3[] trailPoints = null;
    }
}
