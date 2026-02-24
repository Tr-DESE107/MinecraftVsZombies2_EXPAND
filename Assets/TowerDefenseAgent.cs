using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TowerDefenseAgent : Agent
{
    // 这个 Agent 负责代表玩家，观察战场状态并做出决策（放置器械等）

    public override void CollectObservations(VectorSensor sensor)
    {
        // 在这里收集游戏状态，输出长度为 100 的 float 数组
        // 你需要根据你的游戏实际设计状态表示，这里只是一个示例框架

        // 示例：假设场景中有一些怪物和器械，你需要收集它们的类型、位置、血量等信息
        // 为了方便，我们先填充一些随机数据占位，确保维度为 100
        for (int i = 0; i < 100; i++)
        {
            sensor.AddObservation(0f); // 先填充0，后面替换为真实数据
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 接收动作，actions.DiscreteActions[0] 是一个整数（0~21）
        int action = actions.DiscreteActions[0];

        // 根据 action 执行对应的操作，例如放置某种器械
        Debug.Log("Received action: " + action);

        // 在这里调用游戏逻辑，放置器械等
        // 放置后，根据游戏进展给予奖励（可放在其他地方）

        // 示例：简单奖励
        // AddReward(0.1f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 手动控制模式，用于调试：按键盘数字键 0-9 模拟动作
        var discreteActions = actionsOut.DiscreteActions;
        if (Input.GetKeyDown(KeyCode.Alpha0)) discreteActions[0] = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha1)) discreteActions[0] = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) discreteActions[0] = 2;
        // ... 可以继续到 9，但动作空间有22个，你可以扩展用其他按键
    }
}