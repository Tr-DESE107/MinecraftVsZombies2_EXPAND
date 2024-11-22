using System;
using MVZ2.UI;
using Tools;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public class MovingBlueprint : MonoBehaviour
    {
        public void SetMotion(Vector3 startPosition, Transform targetTransform)
        {
            transition.setStartPosition(startPosition);
            transition.setTargetTransform(targetTransform);
            transition.enabled = true;
            moving = true;
        }
        public void SetMotion(Vector3 startPosition, Vector3 targetPosition)
        {
            transition.setStartPosition(startPosition);
            transition.setTargetPosition(targetPosition);
            transition.enabled = true;
            moving = true;
        }
        public void SetBlueprint(Blueprint blueprint)
        {
            this.blueprint = blueprint;
            blueprint.transform.SetParent(transform, false);
            blueprint.transform.localPosition = Vector3.zero;
        }
        private void Update()
        {
            if (moving)
            {
                transition.time += Time.deltaTime * moveSeed;
                if (transition.time >= 1)
                {
                    OnMotionFinished?.Invoke();
                    moving = false;
                }
            }
        }
        public event Action OnMotionFinished;
        [SerializeField]
        private bool moving;
        [SerializeField]
        private float moveSeed;
        [SerializeField]
        private PositionTransition transition;
        [SerializeField]
        private Blueprint blueprint;
    }
}
