using UnityEngine;
using Unity.MLAgents;

public class MLAgentsValidation : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== ML-Agents 验证开始 ===");

        // 通过检查 Agent 类型是否存在来确认 ML-Agents 已安装
        if (typeof(Agent) != null)
        {
            Debug.Log("ML-Agents 命名空间可用，包安装成功！");
        }
        else
        {
            Debug.LogError("ML-Agents 包似乎未正确安装");
            return;
        }

        // 检查 Academy 是否可用（通过 Instance 属性）
        if (Academy.IsInitialized)
        {
            Debug.Log("Academy 已初始化");
        }
        else
        {
            Debug.Log("Academy 尚未初始化（运行时将自动创建）");
        }

        Debug.Log("=== ML-Agents 验证完成 ===");
    }
}