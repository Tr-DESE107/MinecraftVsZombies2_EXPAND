using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.Models
{
    public class NightmareaperModel : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            leftEyeTrail.Init();
            rightEyeTrail.Init();
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            leftEyeTrail.UpdateLogic();
            rightEyeTrail.UpdateLogic();
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var camera = Model.GetCamera();
            UpdateRage(camera);
            leftEyeTrail.UpdateFrame();
            rightEyeTrail.UpdateFrame();
        }
        private void UpdateRage(Camera camera)
        {
            Vector2 leftDir = Main.IsMobile() ? mobileRageLeftArmDir.normalized : rageLeftArmDir.normalized;
            Vector2 rightDir = Main.IsMobile() ? mobileRageRightArmDir.normalized : rageRightArmDir.normalized;

            var rageState = Model.GetProperty<int>("RageState");
            var rageProgress = Model.GetProperty<float>("RageProgress");
            switch (rageState)
            {
                case 1:
                    SetRagingArm(camera, leftArmParent, Vector2.left, leftDir, rageProgress);
                    SetRagingArm(camera, rightArmParent, Vector2.right, rightDir, rageProgress);
                    break;
                case 2:
                    SetClosingArm(camera, leftArmParent, leftDir, rageProgress);
                    SetClosingArm(camera, rightArmParent, rightDir, rageProgress);
                    break;
            }
        }
        private void SetRagingArm(Camera camera, Transform arm, Vector2 fromDir, Vector2 dir, float progress)
        {
            Rect worldRect = camera.GetViewRect();
            Vector2 position = arm.transform.position;

            progress = MathTool.EaseIn(progress);
            if (!MathTool.DoRectAndRayIntersect(worldRect, position, dir, out Vector2 targetPoint))
                return;
            // Scale
            Vector2 targetDistance = targetPoint - position;
            float targetScale = Vector3.Dot(targetDistance, dir) / 1.2f;
            Vector3 leftScale = new Vector3(Mathf.Lerp(1, targetScale, progress) / arm.parent.lossyScale.x, 1, 1);
            arm.localScale = leftScale;

            // Angle
            float angle = Mathf.Lerp(0, Vector2.SignedAngle(fromDir, dir), progress);
            Vector3 angles = Vector3.forward * angle;
            arm.localEulerAngles = angles;

        }
        private void SetClosingArm(Camera camera, Transform arm, Vector2 dir, float progress)
        {
            Rect worldRect = camera.GetViewRect();
            Vector2 position = arm.transform.position;

            progress = MathTool.EaseIn(progress);
            Vector2 targetPos = worldRect.center + dir * 0.6f;
            Vector2 targetDistance = targetPos - position;

            float targetScale = Vector3.Dot(targetDistance, dir) / 1.2f;
            if (!MathTool.DoRectAndRayIntersect(worldRect, position, dir, out Vector2 targetPoint))
                return;
            // Scale
            Vector2 fromDistance = targetPoint - position;
            float fromScale = Vector3.Dot(fromDistance, dir) / 1.2f;

            Vector3 leftScale = new Vector3(Mathf.Lerp(fromScale, targetScale, progress) / arm.parent.lossyScale.x, 1, 1);
            arm.localScale = leftScale;

        }
        [SerializeField]
        private Transform leftArmParent;
        [SerializeField]
        private Transform rightArmParent;

        [SerializeField]
        private Vector2 rageRightArmDir;
        [SerializeField]
        private Vector2 rageLeftArmDir;
        [SerializeField]
        private Vector2 mobileRageRightArmDir;
        [SerializeField]
        private Vector2 mobileRageLeftArmDir;

        [SerializeField]
        private TrailController leftEyeTrail;
        [SerializeField]
        private TrailController rightEyeTrail;
    }
}
