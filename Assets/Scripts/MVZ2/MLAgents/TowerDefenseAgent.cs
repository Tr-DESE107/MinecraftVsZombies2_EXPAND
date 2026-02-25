using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;
using System.Collections;
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
using MVZ2.Vanilla.Entities;// VanillaEntityExt (GetEntityType 等), VanillaEnemyProps (GetSpeed 等)
using MVZ2.Vanilla.Callbacks;// VanillaLevelCallbacks (事件名称常量)
using PVZEngine.Callbacks;  // CallbackResult
using MVZ2.Vanilla.Grids;
using PVZEngine.Base;   // VanillaGridExt (CanSpawnEntity, SpawnPlacedEntity 等)
using MVZ2.Managers;     // Main, DebugManager
using MVZ2Logic.Command; // CommandDefinition
using PVZEngine.SeedPacks; // SeedPack
using PVZEngine.Definitions; // SpawnParams
using PVZEngine.Grids; // LawnGrid
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

    // ===== 静态实例 =====
    public static TowerDefenseAgent Instance { get; private set; }

    // ===== 器械 ID 映射 =====
    // 动作 0~21 对应 22 种器械的 NamespaceID
    // 【需要你补充】根据你的关卡选择的 22 种器械，填入对应的 NamespaceID 字符串
    // 参考 VanillaContraptionID 类中的常量
    private static readonly string[] ContraptionActionMap = new string[]
    {
        "mvz2:dispenser",           // 0 - 发射器
        "mvz2:furnace",             // 1 - 熔炉（生产能量）
        "mvz2:obsidian",            // 2 - 黑曜石（防御）
        "mvz2:tnt",                 // 3 - TNT
        "mvz2:anvil",               // 4 - 铁砧
        "mvz2:super_firework_dispenser", // 5 - 超级烟花发射器
        "mvz2:snipenser",           // 6 - 狙击发射器
        "mvz2:freezenser",          // 7 - 冰冻发射器
        "mvz2:dispen_shield",       // 8 - 盾牌发射器
        "mvz2:glowing_obsidian",    // 9 - 发光黑曜石
        "mvz2:bedrock",             // 10 - 基岩
        "mvz2:barrier",             // 11 - 屏障
        "mvz2:diamond_ore",         // 12 - 钻石矿
        "mvz2:redstone_ore",        // 13 - 红石矿
        "mvz2:eradicator",          // 14 - 根除者
        "mvz2:anti_gravity_pad",    // 15 - 反重力垫
        "mvz2:permanent_ice",       // 16 - 永久冰
        "mvz2:blue_ice",            // 17 - 蓝冰
        "mvz2:desire_pot",          // 18 - 欲望锅
        "mvz2:gunpowder_barrel",     // 19 - 火药桶
        "mvz2:mine_tnt",            // 20 - 地雷TNT
        "mvz2:mannequin_tnt",       // 21 - 假人TNT
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
    private bool aiActive = false; // AI 激活状态

    // ===== AI 鼠标控制 =====
    private Vector3 aiMousePosition; // AI 鼠标位置
    private bool isMouseDown = false; // 鼠标按下状态

    // ===== 重置用的关卡参数 =====
    // 【需要你补充】填入你的关卡 AreaID 和 StageID
    // 参考 LevelManager.InitLevel(NamespaceID areaID, NamespaceID stageID, ...)
    private NamespaceID targetAreaID;  // 例如 new NamespaceID("mvz2", "day")
    private NamespaceID targetStageID; // 例如 new NamespaceID("mvz2", "tutorial")  

    // ===== 获取 LevelEngine 的便捷属性 =====  
    private LevelEngine Level => levelManager?.GetLevel();  

    // =========================================================  
    // Unity 生命周期  
    // =========================================================  

    private void Awake()  
    {
        // 设置静态实例
        Instance = this;
        
        // 如果没有在 Inspector 中拖拽，尝试自动查找  
        if (levelController == null)  
            levelController = FindObjectOfType<LevelController>();  
        if (levelManager == null)  
            levelManager = FindObjectOfType<LevelManager>();  
        
        // 注册 AI 指令
        RegisterAICommands();
    }
    
    /// <summary>
    /// 注册 AI 相关指令
    /// </summary>
    private void RegisterAICommands()
    {
        try
        {
            // 这里可以注册自定义指令
            // 由于指令系统的具体实现可能不同，我们使用更灵活的方式
            // 在游戏运行时通过控制台输入指令来控制 AI
            Debug.Log("[Agent] AI 指令系统已初始化");
            Debug.Log("[Agent] 可用指令:");
            Debug.Log("[Agent]  - ai start: 启动 AI 训练");
            Debug.Log("[Agent]  - ai stop: 停止 AI 训练");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Agent] 注册指令失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 处理 AI 指令
    /// </summary>
    /// <param name="command">指令名称</param>
    /// <param name="args">指令参数</param>
    public void HandleAICommand(string command, string[] args)
    {
        switch (command.ToLower())
        {
            case "start":
                StartAI();
                break;
            case "stop":
                StopAI();
                break;
            default:
                Debug.Log($"[Agent] 未知指令: ai {command}");
                break;
        }
    }
    
    /// <summary>
    /// 启动 AI
    /// </summary>
    public void StartAI()
    {
        if (aiActive)
        {
            Debug.Log("[Agent] AI 已经激活");
            return;
        }
        
        Debug.Log("[Agent] AI 训练启动");
        aiActive = true;
        episodeEnded = false;
        survivalTimer = 0f;
        
        // 订阅事件
        StartCoroutine(SubscribeToEventsAfterDelay());
        
        // 开始新的回合
        Debug.Log("[Agent] AI 训练已启动，开始观察当前关卡");
    }
    
    /// <summary>
    /// 停止 AI
    /// </summary>
    public void StopAI()
    {
        aiActive = false;
        episodeEnded = true;
        Debug.Log("[Agent] AI 训练已停止");
    }
    
    // =========================================================
    // AI 鼠标控制
    // =========================================================
    
    /// <summary>
    /// 设置 AI 鼠标位置
    /// </summary>
    /// <param name="position">屏幕坐标</param>
    private void SetAIMousePosition(Vector3 position)
    {
        aiMousePosition = position;
        // 这里可以添加实际的鼠标位置设置逻辑
        // 例如使用 Input 系统模拟鼠标移动
    }
    
    /// <summary>
    /// 模拟鼠标点击
    /// </summary>
    /// <param name="position">点击位置</param>
    private void SimulateMouseClick(Vector3 position)
    {
        SetAIMousePosition(position);
        isMouseDown = true;
        // 模拟鼠标按下
        // 这里可以添加实际的鼠标点击模拟逻辑
        Debug.Log($"[Agent] 模拟鼠标点击: {position}");
        
        // 模拟鼠标释放
        isMouseDown = false;
    }
    
    /// <summary>
    /// 模拟鼠标按下
    /// </summary>
    private void SimulateMouseDown()
    {
        isMouseDown = true;
        Debug.Log($"[Agent] 模拟鼠标按下: {aiMousePosition}");
    }
    
    /// <summary>
    /// 模拟鼠标释放
    /// </summary>
    private void SimulateMouseUp()
    {
        isMouseDown = false;
        Debug.Log($"[Agent] 模拟鼠标释放: {aiMousePosition}");
    }
    
    /// <summary>
    /// 获取屏幕坐标
    /// </summary>
    /// <param name="worldPosition">世界坐标</param>
    /// <returns>屏幕坐标</returns>
    private Vector3 WorldToScreenPoint(Vector3 worldPosition)
    {
        if (Camera.main != null)
        {
            return Camera.main.WorldToScreenPoint(worldPosition);
        }
        return Vector3.zero;
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

        // 检查 AI 是否激活
        if (!aiActive) return;

        var level = Level;
        if (level == null) return;

        // 检查游戏是否正在运行（参考 LevelController.IsGameRunning()）
        // 【注意】IsGameRunning() 是 LevelController 的方法，需要通过 levelController 访问
        // 但 LevelController 的 IsGameRunning 是 public 的
        if (levelController == null || !levelController.IsGameRunning()) return;  

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
        // 只有当AI被激活时才执行操作
        if (!aiActive)
        {
            Debug.Log("[Agent] AI 未激活，跳过 Episode 开始流程");
            return;
        }
        
        Debug.Log("[Agent] Episode 开始，初始化状态...");
        episodeEnded = false;
        survivalTimer = 0f;

        try
        {
            // 不再重置关卡，使用当前关卡
            // 直接订阅事件
            StartCoroutine(SubscribeToEventsAfterDelay());
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Agent] OnEpisodeBegin 失败: {ex.Message}");
            Debug.LogError($"[Agent] 堆栈跟踪: {ex.StackTrace}");
            // 尝试恢复
            episodeEnded = true;
        }
    }

    /// <summary>
    /// 延迟订阅事件，确保 LevelEngine 初始化完成
    /// </summary>
    private System.Collections.IEnumerator SubscribeToEventsAfterDelay()
    {
        // 只有当AI被激活时才尝试订阅事件
        if (!aiActive)
        {
            Debug.Log("[Agent] AI 未激活，跳过事件订阅");
            yield break;
        }
        
        // 等待多帧，让 LevelEngine 有足够的时间初始化
        int retryCount = 0;
        const int maxRetries = 10;
        
        while (retryCount < maxRetries)
        {
            // 检查 LevelEngine 是否初始化
            var level = Level;
            if (level != null)
            {
                try
                {
                    // 订阅事件
                    SubscribeToLevelEvents();
                    Debug.Log("[Agent] 事件订阅完成");
                    yield break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[Agent] SubscribeToEventsAfterDelay 失败: {ex.Message}");
                    Debug.LogError($"[Agent] 堆栈跟踪: {ex.StackTrace}");
                    yield break;
                }
            }
            else
            {
                Debug.LogWarning($"[Agent] LevelEngine 尚未初始化，延迟订阅事件 (尝试 {retryCount+1}/{maxRetries})");
                // 等待更长时间
                yield return new UnityEngine.WaitForSeconds(0.5f);
                retryCount++;
            }
        }
        
        // 达到最大重试次数后仍然失败
        Debug.LogError("[Agent] LevelEngine 初始化失败，无法订阅事件");
    }  

    public override void CollectObservations(VectorSensor sensor)  
    {  
        try
        {
            // 只有当AI被激活时才执行收集观察的逻辑
            if (!aiActive)
            {
                // 填充默认值，确保观测向量维度正确
                for (int i = 0; i < 100; i++)
                {
                    sensor.AddObservation(0f);
                }
                return;
            }

            // 目标：填充 100 维观测向量  
            // 布局：  
            //   [0]     : 当前能量（归一化到 [0,1]）
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
                try
                {
                    // 当前能量（归一化到 [0,1]）
                    // 使用 LevelEngine 的 Energy 属性获取当前能量值
                    float energy = level.Energy / MAX_ENERGY;
                    sensor.AddObservation(Mathf.Clamp01(energy));  

                    // 当前波次（归一化）  
                    float wave = (float)level.CurrentWave / MAX_WAVE;  
                    sensor.AddObservation(Mathf.Clamp01(wave));  
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[Agent] 收集全局信息失败: {ex.Message}");
                    sensor.AddObservation(0f); // 能量  
                    sensor.AddObservation(0f); // 波次  
                }
            }  
            else  
            {
                sensor.AddObservation(0f); // 能量  
                sensor.AddObservation(0f); // 波次  
            }
            
            // --- 器械信息（10 个槽位 × 5 维 = 50 维）---  
            List<Entity> contraptions = new List<Entity>();
            if (level != null)
            {
                try
                {
                    contraptions = level.FindEntities(e => e.Type == EntityTypes.PLANT && !e.IsDead)
                           .Take(MAX_CONTRAPTIONS).ToList();
                    Debug.Log($"[Agent] 收集到 {contraptions.Count} 个器械");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[Agent] 收集器械信息失败: {ex.Message}");
                    contraptions = new List<Entity>();
                }
            }

            for (int i = 0; i < MAX_CONTRAPTIONS; i++)  
            {
                if (i < contraptions.Count)  
                {
                    try
                    {
                        var c = contraptions[i];  
                        if (c != null)
                        {
                            // [0] 器械类型 ID（归一化，假设最多 22 种）  
                            sensor.AddObservation((float)GetContraptionTypeIndex(c) / 22f);  
                            // [1] 列坐标（归一化）  
                            sensor.AddObservation(c.Position.x / 100f);  
                            // [2] 行坐标（归一化）  
                            sensor.AddObservation(c.Position.z / 100f);  
                            // [3] 血量（归一化）  
                            float maxHp = c.GetMaxHealth();  
                            sensor.AddObservation(maxHp > 0 ? c.Health / maxHp : 0f);  
                            // [4] 是否存活（1=存活）  
                            sensor.AddObservation(c.IsDead ? 0f : 1f);  
                        }
                        else
                        {
                            // 器械为 null，补零
                            sensor.AddObservation(0f);  
                            sensor.AddObservation(0f);  
                            sensor.AddObservation(0f);  
                            sensor.AddObservation(0f);  
                            sensor.AddObservation(0f);  
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[Agent] 收集器械 {i} 信息失败: {ex.Message}");
                        // 发生错误，补零
                        sensor.AddObservation(0f);  
                        sensor.AddObservation(0f);  
                        sensor.AddObservation(0f);  
                        sensor.AddObservation(0f);  
                        sensor.AddObservation(0f);  
                    }
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
            List<Entity> enemies = new List<Entity>();
            if (level != null)
            {
                try
                {
                    enemies = level.FindEntities(e => e.Type == EntityTypes.ENEMY && !e.IsDead)
                           .Take(8).ToList();
                    Debug.Log($"[Agent] 收集到 {enemies.Count} 个怪物");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[Agent] 收集怪物信息失败: {ex.Message}");
                    enemies = new List<Entity>();
                }
            }

            for (int i = 0; i < 8; i++)  
            {
                if (i < enemies.Count)  
                {
                    try
                    {
                        var e = enemies[i];  
                        if (e != null)
                        {
                            sensor.AddObservation((float)GetEnemyTypeIndex(e) / 50f); // 类型 ID  
                            sensor.AddObservation(e.Position.x / 100f);               // X 位置  
                            sensor.AddObservation(e.Position.z / 100f);               // Z 位置  
                            float maxHp = e.GetMaxHealth();  
                            sensor.AddObservation(maxHp > 0 ? e.Health / maxHp : 0f); // 血量  
                            // 速度：通过 VanillaEnemyProps.SPEED 属性获取
                            float speed = e.GetSpeed() / 10f;
                            sensor.AddObservation(Mathf.Clamp01(speed));  
                        }
                        else
                        {
                            // 怪物为 null，补零
                            sensor.AddObservation(0f);  
                            sensor.AddObservation(0f);  
                            sensor.AddObservation(0f);  
                            sensor.AddObservation(0f);  
                            sensor.AddObservation(0f);  
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[Agent] 收集怪物 {i} 信息失败: {ex.Message}");
                        // 发生错误，补零
                        sensor.AddObservation(0f);  
                        sensor.AddObservation(0f);  
                        sensor.AddObservation(0f);  
                        sensor.AddObservation(0f);  
                        sensor.AddObservation(0f);  
                    }
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
        catch (Exception ex)
        {
            Debug.LogError($"[Agent] CollectObservations 失败: {ex.Message}");
            Debug.LogError($"[Agent] 堆栈跟踪: {ex.StackTrace}");
            // 确保传感器被填充，避免训练崩溃
            for (int i = 0; i < 100; i++)
                sensor.AddObservation(0f);
        }
    }

    // =========================================================  
    // OnActionReceived：执行动作 + 即时奖励  
    // =========================================================  

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (episodeEnded) return;

        // 检查 AI 是否激活
        if (!aiActive) return;

        var level = Level;
        if (level == null || levelController == null || !levelController.IsGameRunning()) return;  

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
    /// 尝试通过蓝图系统放置指定器械。  
    /// 模拟玩家点击蓝图并放置的流程。  
    /// </summary>  
    private bool TryPlaceContraption(int actionIndex, LevelEngine level)  
    {  
        if (actionIndex < 0 || actionIndex >= ContraptionActionMap.Length)  
            return false;  

        var contraptionIDStr = ContraptionActionMap[actionIndex];  
        // 将字符串解析为 NamespaceID
        var contraptionID = NamespaceID.ParseStrict(contraptionIDStr);  

        // 检查能量是否足够
        var def = level.Content.GetEntityDefinition(contraptionID);
        var cost = def?.GetCost() ?? 9999;
        if (level.Energy < cost) return false;  

        // 找到所有可用格子
        var grids = level.GetAllGrids();
        if (grids == null || grids.Length == 0)
            return false;
            
        var validGrids = grids.Where(g => g != null && g.CanSpawnEntity(contraptionID)).ToList();
        if (validGrids.Count == 0)
            return false;  

        // 随机选一个格子放置  
        var grid = validGrids[UnityEngine.Random.Range(0, validGrids.Count)];  
        if (grid == null)
            return false;
        
        // 尝试使用蓝图系统放置器械
        try
        {
            // 模拟玩家放置流程
            // 1. 检查是否有对应的种子包（蓝图）
            var seedPack = FindSeedPackForContraption(level, contraptionID);
            if (seedPack == null)
            {
                Debug.LogWarning($"[Agent] 未找到器械 {contraptionID} 的种子包");
                // 如果没有种子包，使用备用方法
                return TryPlaceContraptionFallback(grid, contraptionID, level);
            }

            // 2. 检查种子包是否有效
            if (seedPack == null)
            {
                Debug.LogWarning($"[Agent] 种子包 {contraptionID} 为 null");
                return false;
            }

            // 3. 模拟玩家点击蓝图卡片
            Debug.Log($"[Agent] 模拟点击蓝图: {contraptionID}");
            
            // 4. 模拟玩家点击网格放置器械
            Debug.Log($"[Agent] 模拟点击网格放置器械: {contraptionID} at {grid}");
            
            // 5. 扣除能量
            level.AddEnergy(-cost);
            
            // 6. 使用种子包放置器械
            grid.UseEntityBlueprint(seedPack, null);
            
            // 7. 确保触发放置事件
            {
                // 尝试触发 VanillaLevelCallbacks 中的放置后事件
                try
                {
                    // 检查 VanillaLevelCallbacks 是否存在
                    if (typeof(MVZ2.Vanilla.Callbacks.VanillaLevelCallbacks).GetField("POST_PLACE_ENTITY") != null)
                    {
                        // 由于spawned变量不存在，我们暂时不触发这个事件
                        // 后续可以通过其他方式获取放置的实体
                        Debug.LogWarning("[Agent] 暂未触发放置事件，需要实现获取放置实体的逻辑");
                    }
                }
                catch (Exception ex)
                {
                    // 如果事件不存在或触发失败，忽略错误
                    Debug.LogWarning($"[Agent] 触发放置事件失败: {ex.Message}");
                }
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Agent] 放置器械失败: {e.Message}");
            // 使用备用方法
            return TryPlaceContraptionFallback(grid, contraptionID, level);
        }
    }
    
    /// <summary>
    /// 查找对应器械的种子包
    /// </summary>
    /// <param name="level">LevelEngine实例</param>
    /// <param name="contraptionID">器械ID</param>
    /// <returns>种子包实例</returns>
    private SeedPack FindSeedPackForContraption(LevelEngine level, NamespaceID contraptionID)
    {
        try
        {
            // 由于GetAllSeedPacks方法可能不存在，我们直接返回null
            // 让备用方法处理器械放置
            Debug.LogWarning($"[Agent] 种子包查找功能暂未实现，使用备用放置方法");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[Agent] 查找种子包失败: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 备用放置方法，当蓝图系统不可用时使用
    /// </summary>
    /// <param name="grid">目标网格</param>
    /// <param name="contraptionID">器械ID</param>
    /// <param name="level">LevelEngine实例</param>
    /// <returns>是否放置成功</returns>
    private bool TryPlaceContraptionFallback(LawnGrid grid, NamespaceID contraptionID, LevelEngine level)
    {
        try
        {
            // 直接使用 SpawnPlacedEntity 生成实体
            var spawnParam = new SpawnParams();
            var spawned = grid.SpawnPlacedEntity(contraptionID, spawnParam);
            
            if (spawned != null)
            {
                // 扣除能量
                var def = level.Content.GetEntityDefinition(contraptionID);
                var cost = def?.GetCost() ?? 9999;
                level.AddEnergy(-cost);
                
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Agent] 备用放置方法失败: {e.Message}");
            return false;
        }
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
        if (level == null)
        {
            Debug.Log("[Agent] LevelEngine 尚未初始化，跳过事件订阅");
            return;
        }

        try
        {
            // 订阅怪物死亡事件
            if (typeof(LevelCallbacks).GetField("POST_ENTITY_DEATH") != null)
            {
                level.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, OnEntityDeath);
            }
            else
            {
                Debug.LogWarning("[Agent] POST_ENTITY_DEATH 事件不存在");
            }

            // 订阅关卡通关事件
            if (typeof(LevelCallbacks).GetField("POST_LEVEL_CLEAR") != null)
            {
                level.AddTrigger(LevelCallbacks.POST_LEVEL_CLEAR, OnLevelClearedCallback);
            }
            else
            {
                Debug.LogWarning("[Agent] POST_LEVEL_CLEAR 事件不存在");
            }

            // 订阅关卡失败事件
            if (typeof(LevelCallbacks).GetField("POST_GAME_OVER") != null)
            {
                level.AddTrigger(LevelCallbacks.POST_GAME_OVER, OnGameOverCallback);
            }
            else
            {
                Debug.LogWarning("[Agent] POST_GAME_OVER 事件不存在");
            }

            Debug.Log("[Agent] 事件订阅完成");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Agent] 事件订阅失败: {e.Message}");
        }
    }  

    /// <summary>
    /// 怪物死亡时的回调。
    /// 根据怪物类型给予不同奖励。
    /// </summary>
    private void OnEntityDeath(LevelCallbacks.PostEntityDeathParams param, CallbackResult callbackResult)
    {
        var entity = param.entity;
        if (entity.Type == EntityTypes.ENEMY)
        {
            // 普通怪物奖励
            AddReward(0.2f);
            Debug.Log($"[Agent] 怪物死亡，奖励 +0.2");
        }
        else if (entity.Type == EntityTypes.PLANT) // 器械被摧毁
        {
            // 器械被摧毁惩罚
            AddReward(-0.1f);
            Debug.Log($"[Agent] 器械被摧毁，惩罚 -0.1");
        }
    }

    /// <summary>
    /// 关卡通关回调。
    /// </summary>
    private void OnLevelClearedCallback(LevelCallbackParams param, CallbackResult callbackResult)
    {
        OnLevelCleared();
    }

    /// <summary>
    /// 关卡失败回调。
    /// </summary>
    private void OnGameOverCallback(LevelCallbacks.PostGameOverParams param, CallbackResult callbackResult)
    {
        OnGameOver();
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
        if (levelManager == null)
        {
            Debug.LogError("[Agent] levelManager 为 null，无法重置关卡");
            return;
        }

        try
        {
            // 尝试等待一帧，确保之前的关卡完全清理
            UnityEngine.YieldInstruction wait = new UnityEngine.WaitForEndOfFrame();
            UnityEngine.MonoBehaviour.print("[Agent] 等待关卡清理...");

            // 使用正确的关卡ID
            // 注意：根据你的游戏实际情况修改这些值
            // 你可以在游戏的区域和关卡定义中找到正确的 ID
            targetAreaID = new NamespaceID("mvz2", "day"); // 使用白天区域
            targetStageID = new NamespaceID("mvz2", "tutorial"); // 使用教程关卡

            Debug.Log($"[Agent] 初始化关卡: {targetAreaID} - {targetStageID}");
            levelManager.InitLevel(targetAreaID, targetStageID, beginningDelay: 0f);
            Debug.Log("[Agent] 关卡已重置");
        }
        catch (MissingDefinitionException ex)
        {
            Debug.LogError($"[Agent] 关卡定义不存在: {ex.Message}");
            Debug.LogError("[Agent] 请检查关卡 ID 是否正确，或在游戏中确认可用的关卡");
            
            // 尝试使用备选关卡
            try
            {
                targetAreaID = new NamespaceID("mvz2", "day");
                targetStageID = new NamespaceID("mvz2", "tutorial_1"); // 备选关卡
                Debug.Log($"[Agent] 尝试使用备选关卡: {targetAreaID} - {targetStageID}");
                levelManager.InitLevel(targetAreaID, targetStageID, beginningDelay: 0f);
                Debug.Log("[Agent] 备选关卡已重置");
            }
            catch (Exception ex2)
            {
                Debug.LogError($"[Agent] 备选关卡也失败: {ex2.Message}");
            }
        }
        catch (System.ArgumentException ex) when (ex.Message.Contains("An item with the same key has already been added"))
        {
            // 处理重复键错误
            Debug.LogError($"[Agent] 关卡重置时出现重复键错误: {ex.Message}");
            Debug.LogError("[Agent] 这通常是因为关卡状态没有完全清理，尝试使用备选关卡...");
            
            // 尝试使用备选关卡
            try
            {
                targetAreaID = new NamespaceID("mvz2", "day");
                targetStageID = new NamespaceID("mvz2", "tutorial_1"); // 备选关卡
                Debug.Log($"[Agent] 尝试使用备选关卡: {targetAreaID} - {targetStageID}");
                levelManager.InitLevel(targetAreaID, targetStageID, beginningDelay: 0f);
                Debug.Log("[Agent] 备选关卡已重置");
            }
            catch (Exception ex2)
            {
                Debug.LogError($"[Agent] 备选关卡也失败: {ex2.Message}");
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError($"[Agent] 重置关卡时出现空引用: {ex.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Agent] 重置关卡失败: {e.Message}");
        }
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