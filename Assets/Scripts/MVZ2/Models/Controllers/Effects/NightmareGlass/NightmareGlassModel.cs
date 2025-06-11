using MVZ2.Models;
using UnityEngine;

namespace MVZ2
{
    public class NightmareGlassModel : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            var infos = new NightmareGlassShardInfo[shards.Length];
            for (int i = 0; i < infos.Length; i++)
            {
                var shard = shards[i];
                infos[i] = new NightmareGlassShardInfo()
                {
                    position = shard.localPosition,
                    rotation = shard.eulerAngles,
                    scale = shard.localScale,
                };
            }
            Model.SetProperty("Infos", infos);
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var infos = Model.GetProperty<NightmareGlassShardInfo[]>("Infos");
            for (int i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                info.position += info.velocity;
                info.rotation += info.angularSpeed;
                info.scale += info.scaleSpeed;
            }
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var infos = Model.GetProperty<NightmareGlassShardInfo[]>("Infos");
            for (int i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                var shard = shards[i];
                shard.localPosition = info.position;
                shard.eulerAngles = info.rotation;
                shard.localScale = info.scale;
            }
        }
        public override void OnTrigger(string name)
        {
            base.OnTrigger(name);
            if (name == "Break")
            {
                BreakShards();
            }
        }
        private void BreakShards()
        {
            var infos = Model.GetProperty<NightmareGlassShardInfo[]>("Infos");
            var rng = Model.GetRNG();
            for (int i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                info.velocity = info.position * 0.2f;
                info.angularSpeed = new Vector3(rng.Next(-maxRotationSpeed, maxRotationSpeed), rng.Next(-maxRotationSpeed, maxRotationSpeed), rng.Next(-maxRotationSpeed, maxRotationSpeed));
                info.scaleSpeed = Vector3.one * -1 / 60f;
            }
        }

        [SerializeField]
        private float maxRotationSpeed = 30f;
        [SerializeField]
        private Transform[] shards;
    }
    [SerializeField]
    public class NightmareGlassShardInfo
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public Vector3 velocity;
        public Vector3 angularSpeed;
        public Vector3 scaleSpeed;
    }
}
