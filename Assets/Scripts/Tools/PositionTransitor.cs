using UnityEngine;
namespace Tools
{
    [DisallowMultipleComponent]
    public abstract class PositionTransitor : MonoBehaviour
    {
        #region 公有方法

        #region 开始位置
        public void setStartPositionToCurrent()
        {
            setStartPosition(transform.position);
        }
        public void setStartTransform(Transform transform)
        {
            _startTransform = transform;
            updatePosition();
        }
        public void setStartPosition(Vector3 globalPosition)
        {
            if (isLocalMode)
            {
                _startPosition = globalToLocalPoint(globalPosition);
            }
            else
            {
                _startPosition = globalPosition;
            }
            updatePosition();
        }
        public void setStartLocalPosition(Vector3 localPosition)
        {
            if (isLocalMode)
            {
                _startPosition = localPosition;
            }
            else
            {
                _startPosition = localToGlobalPoint(localPosition);
            }
            updatePosition();
        }
        #endregion

        #region 目标位置
        public void setTargetTransform(Transform target)
        {
            _targetTransform = target;
            _targetPosition = Vector3.zero;
            updatePosition();
        }
        public void setTargetPosition(Vector3 targetPos)
        {
            _targetTransform = null;
            _targetPosition = targetPos;
            updatePosition();
        }
        public void setTargetPositionToCurrent()
        {
            setTargetPosition(transform.position);
        }
        #endregion

        #region 当前位置
        public void setWorldPosition(Vector3 pos)
        {
            transform.position = pos;
        }
        public Vector3 getWorldPosition()
        {
            return transform.position;
        }
        public void setLocalPosition(Vector3 pos)
        {
            transform.localPosition = pos;
        }
        public Vector3 getLocalPosition()
        {
            return transform.localPosition;
        }
        public void setPositionByTime(float time)
        {
            if (isLocalMode)
            {
                Vector3 targetPosition = targetTransform ? targetTransform.position : _targetPosition;
                Vector3 globalStartPosition;
                Vector3 localStartPosition;
                if (startTransform)
                {
                    globalStartPosition = startTransform.position;
                    localStartPosition = globalToLocalPoint(startTransform.position);
                }
                else
                {
                    globalStartPosition = localToGlobalPoint(_startPosition);
                    localStartPosition = _startPosition;
                }
                targetPosition = globalToLocalPoint(targetPosition);

                var position = Transit(localStartPosition, targetPosition, time);
                setLocalPosition(position);
            }
            else
            {
                Vector3 startPosition = startTransform ? startTransform.position : _startPosition;
                Vector3 targetPosition = targetTransform ? targetTransform.position : _targetPosition;

                var position = Transit(startPosition, targetPosition, time);
                setWorldPosition(position);
            }
        }
        public void setPositionToStart()
        {
            setPositionByTime(0);
        }
        public void updatePosition()
        {
            setPositionByTime(time);
        }
        #endregion

        #endregion

        #region 私有方法

        #region 生命周期
        protected void Reset()
        {
            enabled = false;
        }
        protected void OnEnable()
        {
            initStartPosition();
        }
        protected void LateUpdate()
        {
            updatePosition();
        }
        #endregion

        protected abstract Vector3 Transit(Vector3 start, Vector3 end, float time);
        private void initStartPosition()
        {
            if (!Application.isPlaying)
                return;
            setStartPositionToCurrent();
        }
        private Vector3 globalToLocalPoint(Vector3 global)
        {
            if (!transform.parent)
            {
                return global;
            }
            return transform.parent.InverseTransformPoint(global);
        }
        private Vector3 localToGlobalPoint(Vector3 local)
        {
            if (!transform.parent)
            {
                return local;
            }
            return transform.parent.TransformPoint(local);
        }
        #endregion

        #region 属性字段
        public Transform startTransform => _startTransform;
        public Transform targetTransform => _targetTransform;
        [Range(0, 1)]
        [SerializeField]
        public float time;
        [SerializeField]
        bool isLocalMode;
        [SerializeField]
        Transform _startTransform;
        [SerializeField]
        Transform _targetTransform;
        [SerializeField]
        Vector3 _startPosition;
        [SerializeField]
        Vector3 _targetPosition;
        public bool IsLocalMode
        {
            get => isLocalMode;
            set => isLocalMode = value;
        }
        public Vector3 startPosition
        {
            get => _startPosition;
        }
        #endregion
    }
}
