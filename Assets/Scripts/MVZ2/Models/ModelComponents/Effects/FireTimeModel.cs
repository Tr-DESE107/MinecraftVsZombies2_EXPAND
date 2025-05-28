namespace MVZ2.Models
{
    public class FireTimeModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            fireTime += deltaTime;
            Model.SetShaderFloat("_FireTime", fireTime);
        }
        private float fireTime = 0;
    }
}
