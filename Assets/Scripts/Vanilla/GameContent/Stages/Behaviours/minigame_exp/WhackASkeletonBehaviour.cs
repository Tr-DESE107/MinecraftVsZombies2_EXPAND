// ������Ϸ�߼���������������ռ�
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.HeldItems;
using MVZ2.Vanilla.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using MVZ2Logic.HeldItems;

namespace MVZ2.GameContent.Stages
{
    /// <summary>
    /// �������顱С��Ϸ�Ĺؿ���Ϊ�߼���Whack-A-Ghost��
    /// </summary>
    public class WhackASkeletonBehaviour : StageBehaviour
    {
        public WhackASkeletonBehaviour(StageDefinition stageDef) : base(stageDef) { }

        /// <summary>
        /// �ؿ���ʼʱ�����������׵綨ʱ����ÿ150֡һ�Σ�
        /// </summary>
        public override void Start(LevelEngine level)
        {
            //SetThunderTimer(level, new FrameTimer(150));
        }

        /// <summary>
        /// ÿ֡���£��Զ�����ҷ�������ʱ�����׵�
        /// </summary>
        public override void Update(LevelEngine level)
        {
            // �����ǰδ������Ʒ�������ҷ�һ�ѽ�
            if (level.GetHeldItemType() == BuiltinHeldTypes.none)
            {
                var builder = new HeldItemBuilder(VanillaHeldTypes.sword);
                builder.SetCannotCancel(true);
                level.SetHeldItem(builder);
            }

            //// �����׵��ʱ��
            //var timer = GetThunderTimer(level);
            //timer.Run();

            //// �����ʱ�����ڣ��򴥷�һ���׵粢���ü�ʱ��
            //if (timer.Expired)
            //{
            //    level.Thunder(); // ��մ���
            //    timer.Reset();   // ���ü�ʱ��
            //}
        }

        /// <summary>
        /// ÿ�������󴥷�
        /// - ������һ���׵����ȴʱ��
        /// - ���� Napstablook �������
        /// </summary>
        public override void PostWave(LevelEngine level, int wave)
        {
            base.PostWave(level, wave);

            // �����׵���ȴʱ�䣨���30֡һ�Σ�
            //var timer = GetThunderTimer(level);
            //timer.Frame = Mathf.Min(timer.Frame, 30);

            // ���ݵ�ǰ������������ Napstablook ��������5��֮��ſ�ʼ��
            var napstablookPoints = (wave - 5) / 3f;

            // ����Ǵ��ͽ���������������
            if (level.IsHugeWave(wave))
            {
                napstablookPoints *= 2.5f;
            }

            // ����ȡ��Ϊ��������
            var napstablookCount = Mathf.CeilToInt(napstablookPoints);

            for (int i = 0; i < napstablookCount; i++)
            {
                // ���ѡ��һ������·����һ��
                var lane = level.GetRandomEnemySpawnLane();
                var column = level.GetSpawnRNG().Next(0, 5);

                // ��ȡ��Ӧλ������
                var x = level.GetColumnX(column);
                var z = level.GetEntityLaneZ(lane);
                var y = level.GetGroundY(x, z);

                // �������ɲ�����������ӪΪ������Ӫ
                var spawnParam = new SpawnParams();
                spawnParam.SetProperty(EngineEntityProps.FACTION, level.Option.LeftFaction);

                // ���� Napstablook ���������� Buff
                //var napstablook = level.Spawn(VanillaEnemyID.napstablook, new Vector3(x, y, z), null, spawnParam);
                //AddSpeedBuff(napstablook);
            }
        }

        /// <summary>
        /// �������ɺ󴥷���������ǰλ�ƣ��������е���
        /// </summary>
        public override void PostEnemySpawned(Entity entity)
        {
            base.PostEnemySpawned(entity);

            // ��������飨ghost���������ǰ�ƽ�һ�ξ���
            //if (entity.IsEntityOf(VanillaEnemyID.ghost))
            //{
            //    var advanceDistance = entity.RNG.Next(0, entity.Level.GetGridWidth() * 3f);
            //    entity.Position += Vector3.left * advanceDistance;
            //}

            // ���е�������ٶ� Buff������ Napstablook �� ghost��
            AddSpeedBuff(entity);
        }

        /// <summary>
        /// ����������ٶ� Buff���ٶȱ����沨������
        /// </summary>
        private void AddSpeedBuff(Entity entity)
        {
            var buff = entity.AddBuff<MinigameEnemySpeedBuff>();
            float speedMultiplier = Mathf.Lerp(3, 5, entity.Level.CurrentWave / (float)entity.Level.GetTotalWaveCount());
            buff.SetProperty(MinigameEnemySpeedBuff.PROP_SPEED_MULTIPLIER, speedMultiplier);
        }

        #region �ؿ�����

        //// �����׵��ʱ���ֶ�ֵ
        //private void SetThunderTimer(LevelEngine level, FrameTimer timer)
        //    => level.SetBehaviourField(PROP_THUNDER_TIMER, timer);

        //// ��ȡ�׵��ʱ���ֶ�ֵ
        //private FrameTimer GetThunderTimer(LevelEngine level)
        //    => level.GetBehaviourField<FrameTimer>(PROP_THUNDER_TIMER);

        #endregion

        #region �����ֶζ���

        // ����ע�������������ڹؿ����л���
        //private const string PROP_REGION = "whack_a_ghost";

        //// ע��һ���ؿ����ԣ��׵��ʱ��
        //[LevelPropertyRegistry(PROP_REGION)]
        //public static readonly VanillaLevelPropertyMeta<FrameTimer> PROP_THUNDER_TIMER =
        //    new VanillaLevelPropertyMeta<FrameTimer>("ThunderTimer");

        #endregion
    }
}
