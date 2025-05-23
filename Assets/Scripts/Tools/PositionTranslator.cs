﻿using UnityEngine;
namespace Tools
{
    [ExecuteInEditMode]
    public class PositionTransition : PositionTransitor
    {
        public void SetStopDistance(float stopDistance)
        {
            _stopDistance = stopDistance;
        }
        protected override Vector3 Transit(Vector3 start, Vector3 end, float time)
        {
            Vector3 stoppedPosition = Vector3.MoveTowards(end, start, _stopDistance);
            return Vector3.Lerp(start, stoppedPosition, time);
        }
        [SerializeField]
        float _stopDistance;
    }
}
