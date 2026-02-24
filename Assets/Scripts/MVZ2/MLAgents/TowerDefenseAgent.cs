using UnityEngine;  
using Unity.MLAgents;  
using Unity.MLAgents.Sensors;  
using Unity.MLAgents.Actuators;  
using System.Collections.Generic;  
using System.Linq;  
  
// ========== 需要补充的命名空间 ==========
// 根据 MVZ2 代码库，你需要引用以下命名空间（请确认实际路径）：
using PVZEngine;             // NamespaceID
using MVZ2.Level;           // LevelController, LevelManager
using MVZ2Logic.Level;      // LevelEngine
using PVZEngine.Level;      // LevelEngine 基类
using PVZEngine.Entities;   // Entity, EntityTypes
using MVZ2.Vanilla.Level;   // VanillaLevelExt (CheckGameOver, IsCleared 等)
using MVZ2.Vanilla.Entities;// VanillaEntityExt (GetEntityType 等)
using MVZ2.Vanilla.Callbacks;// VanillaLevelCallbacks (事件名称常量)
using PVZEngine.Callbacks;  // CallbackResult
using MVZ2.Vanilla.Grids;   // VanillaGridExt (CanSpawnEntity, SpawnPlacedEntity 等)
// =========================================  
  
/// <summary>  
/// TowerDefenseAgent：ML-Agents Agent，负责观察战场并决策放置器械。  
///   
/// 【重要】此脚本需要挂载在场景中的一个 GameObject 上，  
/// 并且需要能访问到 LevelController 和 LevelManager。  
/// 建议通过 Inspector 拖拽或 FindObjectOfType 获取引用。  
/// </summary>  
public class TowerDefenseAgent : Agent  
{  
    // ===== Inspector 可配置引用 =====  
    [Header("游戏管理器引用")]  
    [Tooltip("拖拽场景中的 LevelController GameObject")]  
    public LevelController levelController; // MVZ2.Level.LevelController  
  
    [Tooltip("拖拽场景中的 LevelManager GameObject")]  
    public LevelManager levelManager;       // MVZ2.Level.LevelManager  
  
    // ===== 器械 ID 映射 =====  
    // 动作 0~21 对应 22 种器械的 NamespaceID  
    // 【需要你补充】根据你的关卡选择的 22 种器械，填入对应的 NamespaceID 字符串  
    // 参考 VanillaContraptionID 类中的常量  
    private static readonly string[] ContraptionActionMap = new string[]  
    {  
        "mvz2:dispenser",       // 0 - 发射器  
        "mvz2:furnace",         // 1 - 熔炉（生产能量）  
        "mvz2:obsidian",        // 2 - 黑曜石（防御）  
        "mvz2:tnt",             // 3 - TNT  
        "mvz2:anvil",           // 4 - 铁砧（来自 entities.xml）  
        // ... 继续填充到 21  
        // 【TODO】根据你的 22 种器械补全此数组  
        "mvz2:dispenser",       // 5 (占位)  
        "mvz2:dispenser",       // 6 (占位)  
        "mvz2:dispenser",       // 7 (占位)  
        "mvz2:dispenser",       // 8 (占位)  
        "mvz2:dispenser",       // 9 (占位)  
        "mvz2:dispenser",       // 10 (占位)  
        "mvz2:dispenser",       // 11 (占位)  
        "mvz2:dispenser",       // 12 (占位)  
        "mvz2:dispenser",       // 13 (占位)  
        "mvz2:dispenser",       // 14 (占位)  
        "mvz2:dispenser",       // 15 (占位)  
        "mvz2:dispenser",       // 16 (占位)  
        "mvz2:dispenser",       // 17 (占位)  
        "mvz2:dispenser",       // 18 (占位)  
        "mvz2:dispenser",       // 19 (占位)  
        "mvz2:dispenser",       // 20 (占位)  
        "mvz2:dispenser",       // 21 (占位)  
    };  
  
    // ===== 观测参数 =====  
    private const int MAX_CONTRAPTIONS = 10;  // 最多观测 10 个器械  
    private const int MAX_ENEMIES = 10;        // 最多观测 10 个怪物  
    private const int FLOATS_PER_ENTITY = 5;  // 每个实体 5 个 float  
    private const float MAX_ENERGY = 9990f;   // 能量上限（来自 Readme）  
    private const float MAX_WAVE = 20f;        // 最大波次（根据你的关卡调整）  
  
    // ===== 存活时间奖励 =====  
    private float survivalTimer = 0f;  
    private const float SURVIVAL_REWARD_INTERVAL = 10f; // 每 10 秒给一次存活奖励  
  
    // ===== 状态标志 =====  
    private bool episodeEnded = false;  
  
    // ===== 重置用的关卡参数 =====
    // 【需要你补充】填入你的关卡 AreaID 和 StageID
    // 参考 LevelManager.InitLevel(NamespaceID areaID, NamespaceID stageID, ...)
    private NamespaceID targetAreaID;  // 例如 new NamespaceID("mvz2", "castle")
    private NamespaceID targetStageID; // 例如 new NamespaceID("mvz2", "castle_1")  
  
    // ===== 获取 LevelEngine 的便捷属性 =====  
    private LevelEngine Level => levelManager?.GetLevel();  
  
    // =========================================================  
    // Unity 生命周期  
    // =========================================================  
  
    private void Awake()  
    {  
        // 如果没有在 Inspector 中拖拽，尝试自动查找  
        if (levelController == null)  
            levelController = FindObjectOfType<LevelController>();  
        if (levelManager == null)  
            levelManager = FindObjectOfType<LevelManager>();  
    }  
  
    private void Start()  
    {  
        // 订阅游戏事件  
        // 注意：事件订阅需要在 LevelEngine 初始化后进行  
        // 建议在 OnEpisodeBegin 中或等待关卡加载完成后再订阅  
        SubscribeToLevelEvents();  
    }  
  
    private void Update()  
    {  
        if (episodeEnded) return;  
  
        var level = Level;  
        if (level == null) return;  
  
        // 检查游戏是否正在运行（参考 LevelController.IsGameRunning()）  
        // 【注意】IsGameRunning() 是 LevelController 的方法，需要通过 levelController 访问  
        // 但 LevelController 的 IsGameRunning 是 public 的  
        if (!levelController.IsGameRunning()) return;  
  
        // 存活时间奖励：每 10 秒 +0.01  
        survivalTimer += Time.deltaTime;  
        if (survivalTimer >= SURVIVAL_REWARD_INTERVAL)  
        {  
            survivalTimer -= SURVIVAL_REWARD_INTERVAL;  
            AddReward(0.01f);  
            Debug.Log("[Agent] 存活奖励 +0.01");  
        }  
  
        // 检测失败条件：僵尸进家  
        // VanillaLevelExt.CheckGameOver() 会在僵尸越过边界时调用 level.GameOver()  
        // 我们通过 level.IsCleared 和 isGameOver 标志来检测  
        // 【注意】isGameOver 是 LevelController 的私有字段，需要通过公开方法访问  
        // 可以检查 level.IsCleared 属性（LevelEngine 上的属性）  
        if (level.IsCleared)  
        {  
            // 关卡通关  
            OnLevelCleared();  
        }  
    }  
  
    // =========================================================  
    // ML-Agents 核心方法  
    // =========================================================  
  
    public override void OnEpisodeBegin()  
    {  
        Debug.Log("[Agent] Episode 开始，重置游戏...");  
        episodeEnded = false;  
        survivalTimer = 0f;  
  
        // 重置游戏关卡  
        ResetLevel();  
  
        // 重新订阅事件（因为 LevelEngine 可能已重建）  
        SubscribeToLevelEvents();  
    }  
  
    public override void CollectObservations(VectorSensor sensor)  
    {  
        // 目标：填充 100 维观测向量  
        // 布局：  
        //   [0]     : 当前能量（归一化）  
        //   [1]     : 当前波次（归一化）  
        //   [2]     : 是否为旗帜波（0/1）  
        //   [3]     : 游戏是否运行中（0/1）  
        //   [4~53]  : 最多 10 个器械，每个 5 个 float（共 50 维）  
        //   [54~103]: 最多 10 个怪物，每个 5 个 float（共 50 维）  
        // 总计：4 + 50 + 50 = 104 维 → 需要调整为 100 维，可减少实体槽位  
        // 【当前方案】4 全局 + 48 器械(9个×5+3) + 48 怪物 → 建议调整为：  
        //   2 全局 + 10器械×5 + 10怪物×5 = 2 + 50 + 50 = 102 → 去掉2个全局变量  
        //   或：2 全局 + 9器械×5 + 9怪物×5 = 2 + 45 + 45 = 92 → 补8个0  
        // 【最终方案】保持 100 维：2 全局 + 10器械×5 + 8怪物×5 = 2+50+40=92，补8个0  
        // 你可以根据需要调整，只要总维度 = 100  
  
        var level = Level;  
  
        // --- 全局信息（2 维）---  
        if (level != null)  
        {  
            // 当前能量（归一化到 [0,1]）  
            // 【需要补充】level.Energy 或通过 VanillaLevelExt 扩展方法获取  
            // 参考：level.GetProperty<int>(VanillaLevelProps.ENERGY) 或类似方式  
            float energy = 0f;  
            // TODO: energy = level.GetEnergy() / MAX_ENERGY;  
            // 暂时用 0 占位，你需要找到获取能量的正确 API  
            sensor.AddObservation(energy);  
  
            // 当前波次（归一化）  
            float wave = (float)level.CurrentWave / MAX_WAVE;  
            sensor.AddObservation(Mathf.Clamp01(wave));  
        }  
        else  
        {  
            sensor.AddObservation(0f); // 能量  
            sensor.AddObservation(0f); // 波次  
        }
        // --- 器械信息（10 个槽位 × 5 维 = 50 维）---  
        var contraptions = level != null  
            ? level.FindEntities(e => e.Type == EntityTypes.PLANT && !e.IsDead)  
                   .Take(MAX_CONTRAPTIONS).ToList()  
            : new List<Entity>();  
  
        for (int i = 0; i < MAX_CONTRAPTIONS; i++)  
        {  
            if (i < contraptions.Count)  
            {  
                var c = contraptions[i];  
                // [0] 器械类型 ID（归一化，假设最多 22 种）  
                sensor.AddObservation((float)GetContraptionTypeIndex(c) / 22f);  
                // [1] 列坐标（归一化）  
                sensor.AddObservation(c.Position.x / 100f);  
                // [2] 行坐标（归一化）  
                sensor.AddObservation(c.Position.z / 100f);  
                // [3] 血量（归一化）  
                // Entity.Health 是直接属性，MaxHealth 通过 GetMaxHealth() 扩展方法获取  
                float maxHp = c.GetMaxHealth();  
                sensor.AddObservation(maxHp > 0 ? c.Health / maxHp : 0f);  
                // [4] 是否存活（1=存活）  
                sensor.AddObservation(c.IsDead ? 0f : 1f);  
            }  
            else  
            {  
                // 不足时补零  
                sensor.AddObservation(0f);  
                sensor.AddObservation(0f);  
                sensor.AddObservation(0f);  
                sensor.AddObservation(0f);  
                sensor.AddObservation(0f);  
            }  
        }  
  
        // --- 怪物信息（8 个槽位 × 5 维 = 40 维）---  
        // EntityTypes.ENEMY 对应怪物类型  
        var enemies = level != null  
            ? level.FindEntities(e => e.Type == EntityTypes.ENEMY && !e.IsDead)  
                   .Take(8).ToList()  
            : new List<Entity>();  
  
        for (int i = 0; i < 8; i++)  
        {  
            if (i < enemies.Count)  
            {  
                var e = enemies[i];  
                sensor.AddObservation((float)GetEnemyTypeIndex(e) / 50f); // 类型 ID  
                sensor.AddObservation(e.Position.x / 100f);               // X 位置  
                sensor.AddObservation(e.Position.z / 100f);               // Z 位置  
                float maxHp = e.GetMaxHealth();  
                sensor.AddObservation(maxHp > 0 ? e.Health / maxHp : 0f); // 血量  
                // 速度：通过 VanillaEnemyProps.SPEED 属性获取  
                // TODO: sensor.AddObservation(e.GetSpeed() / 10f);  
                sensor.AddObservation(0f); // 占位，替换为真实速度  
            }  
            else  
            {  
                sensor.AddObservation(0f);  
                sensor.AddObservation(0f);  
                sensor.AddObservation(0f);  
                sensor.AddObservation(0f);  
                sensor.AddObservation(0f);  
            }  
        }  
  
        // 补齐到 100 维（2 + 50 + 40 = 92，补 8 个 0）  
        for (int i = 0; i < 8; i++)  
            sensor.AddObservation(0f);  
    }  
  
    // =========================================================  
    // OnActionReceived：执行动作 + 即时奖励  
    // =========================================================  
  
    public override void OnActionReceived(ActionBuffers actions)  
    {  
        if (episodeEnded) return;  
  
        var level = Level;  
        if (level == null || !levelController.IsGameRunning()) return;  
  
        int action = actions.DiscreteActions[0]; // 0~21  
        bool placed = TryPlaceContraption(action, level);  
  
        if (placed)  
        {  
            AddReward(0.1f);  
            Debug.Log($"[Agent] 成功放置器械 {action}，奖励 +0.1");  
        }  
        else  
        {  
            AddReward(-0.05f);  
            Debug.Log($"[Agent] 放置失败（资源不足或位置不可用），惩罚 -0.05");  
        }  
    }  
  
    // =========================================================  
    // 放置器械逻辑  
    // =========================================================  
  
    /// <summary>  
    /// 尝试在随机可用格子上放置指定器械。  
    /// 使用 LawnGrid.CanSpawnEntity() 检查格子是否可用，  
    /// 然后调用 grid.SpawnPlacedEntity() 生成实体。  
    /// 参考：LevelController_Input.OnCommandBlockTestKey() 的实现方式。  
    /// </summary>  
    private bool TryPlaceContraption(int actionIndex, LevelEngine level)  
    {  
        if (actionIndex < 0 || actionIndex >= ContraptionActionMap.Length)  
            return false;  
  
        var contraptionIDStr = ContraptionActionMap[actionIndex];  
        // 将字符串解析为 NamespaceID
        var contraptionID = NamespaceID.ParseStrict(contraptionIDStr);  
  
        // 检查能量是否足够  
        // 【TODO】获取该器械的费用：  
        // var def = level.Content.GetEntityDefinition(contraptionID);  
        // var cost = def?.GetCost() ?? 9999;  
        // if (level.Energy < cost) return false;  
  
        // 找到所有可用格子
        var grids = level.GetAllGrids();
        var validGrids = grids.Where(g => g != null && g.CanSpawnEntity(contraptionID)).ToList();
        if (validGrids.Count == 0)
            return false;  
  
        // 随机选一个格子放置  
        var grid = validGrids[Random.Range(0, validGrids.Count)];  
        var spawnParam = new SpawnParams();  
        var spawned = grid.SpawnPlacedEntity(contraptionID, spawnParam);  
        return spawned != null;  
    }  
  
    // =========================================================  
    // 事件订阅与处理  
    // =========================================================  
  
    /// <summary>  
    /// 订阅 LevelEngine 的 Trigger 事件。  
    /// 参考 LevelController_Sponsors.AddLevelCallbacks_Sponsors() 的写法：  
    ///   level.AddTrigger(VanillaLevelCallbacks.POST_USE_ENTITY_BLUEPRINT, callback);  
    /// </summary>  
    private void SubscribeToLevelEvents()  
    {  
        var level = Level;  
        if (level == null) return;  
  
        // 订阅怪物死亡事件  
        // 【TODO】找到正确的 Callback 常量名，例如：  
        // level.AddTrigger(VanillaLevelCallbacks.POST_ENTITY_DEATH, OnEntityDeath);  
  
        // 订阅器械被摧毁事件  
        // level.AddTrigger(VanillaLevelCallbacks.POST_ENTITY_DEATH, OnContraptionDestroyed);  
  
        // 订阅关卡通关事件  
        // level.AddTrigger(VanillaLevelCallbacks.POST_LEVEL_CLEAR, OnLevelClearedCallback);  
  
        // 订阅关卡失败事件  
        // level.AddTrigger(VanillaLevelCallbacks.POST_GAME_OVER, OnGameOverCallback);  
  
        Debug.Log("[Agent] 事件订阅完成（需补充具体 Callback 常量）");  
    }  
  
    /// <summary>  
    /// 怪物死亡时的回调。  
    /// 根据怪物类型给予不同奖励。  
    /// </summary>  
    private CallbackResult OnEntityDeath(/* 参数类型根据实际 Trigger 签名填写 */)  
    {  
        // 示例：  
        // if (entity.Type == EntityTypes.ENEMY)  
        // {  
        //     bool isElite = entity.HasBuff<SomeBossBuffType>();  
        //     AddReward(isElite ? 0.5f : 0.2f);  
        // }  
        // else if (entity.Type == EntityTypes.PLANT) // 器械被摧毁  
        // {  
        //     AddReward(-0.1f);  
        // }  
        return new CallbackResult(true);  
    }  
  
    /// <summary>  
    /// 关卡通关（胜利）处理。  
    /// level.IsCleared 为 true 时触发。  
    /// </summary>  
    private void OnLevelCleared()  
    {  
        if (episodeEnded) return;  
        episodeEnded = true;  
        Debug.Log("[Agent] 关卡通关！奖励 +1.0");  
        SetReward(1.0f);  
        EndEpisode();  
    }  
  
    /// <summary>  
    /// 关卡失败处理。  
    /// 僵尸进家时（CheckGameOver 检测到）触发。  
    /// </summary>  
    private void OnGameOver()  
    {  
        if (episodeEnded) return;  
        episodeEnded = true;  
        Debug.Log("[Agent] 关卡失败！惩罚 -1.0");  
        SetReward(-1.0f);  
        EndEpisode();  
    }  
  
    // =========================================================  
    // 重置关卡  
    // =========================================================  
  
    /// <summary>
    /// 重置关卡到初始状态。
    /// 调用 LevelManager.InitLevel() 重新加载关卡。
    /// 注意：InitLevel 会触发存档保存，训练时可能需要禁用。
    /// </summary>
    private void ResetLevel()
    {
        if (levelManager == null) return;

        // 【需要你填入】你的关卡 AreaID 和 StageID
        // 例如：
        targetAreaID = new NamespaceID("mvz2", "castle");
        targetStageID = new NamespaceID("mvz2", "castle_1");

        levelManager.InitLevel(targetAreaID, targetStageID, beginningDelay: 0f);
        Debug.Log("[Agent] 关卡已重置");
    }  
  
    // =========================================================  
    // 辅助方法  
    // =========================================================  
  
    /// <summary>  
    /// 获取器械类型的整数索引（用于观测编码）。  
    /// 根据 ContraptionActionMap 中的顺序返回索引。  
    /// </summary>  
    private int GetContraptionTypeIndex(Entity entity)  
    {  
        var id = entity.GetDefinitionID()?.ToString();  
        if (id == null) return 0;  
        var idx = System.Array.IndexOf(ContraptionActionMap, id);  
        return idx >= 0 ? idx : 0;  
    }  
  
    /// <summary>  
    /// 获取怪物类型的整数索引（用于观测编码）。  
    /// 【TODO】根据你的怪物 ID 列表建立映射。  
    /// </summary>  
    private int GetEnemyTypeIndex(Entity entity)  
    {  
        // 简单用 Definition ID 的 hashcode 取模  
        var id = entity.GetDefinitionID()?.ToString() ?? "";  
        return Mathf.Abs(id.GetHashCode()) % 50;  
    }  
  
    public override void Heuristic(in ActionBuffers actionsOut)  
    {  
        var discreteActions = actionsOut.DiscreteActions;  
        // 数字键 0-9 对应动作 0-9  
        for (int i = 0; i <= 9; i++)  
        {  
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))  
            {  
                discreteActions[0] = i;  
                return;  
            }  
        }  
        // F1-F12 对应动作 10-21  
        for (int i = 0; i <= 11; i++)  
        {  
            if (Input.GetKeyDown(KeyCode.F1 + i))  
            {  
                discreteActions[0] = 10 + i;  
                return;  
            }  
        }  
    }  
}